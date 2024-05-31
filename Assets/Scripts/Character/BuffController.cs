using FishNet;
using FishNet.Managing.Logging;
using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class BuffController : NetworkBehaviour {
    private const float MAX_PASSED_TIME = 0.4f;

    private NetworkStats _myStats;

    public event Action<List<Buff>> OnBuffsChanged;

    private List<Buff> _activeBuffs;

    [HideInInspector]
    public bool IsStunned = false;

    [SerializeField]
    private bool _canReceiveBuffs = true;

    public event Action OnStunApplied;

    public event Action OnStunExpired;

    public List<Buff> ActiveBuffs {
        get { return _activeBuffs; }
        set { _activeBuffs = value; OnBuffsChanged?.Invoke(_activeBuffs); }
    }

    private void Awake() {
        _myStats = GetComponent<NetworkStats>();
        ActiveBuffs = new List<Buff>();
    }

    private void OnEnable() {
        OnBuffsChanged += HandleBuffsChanged;
    }

    public void OnDisable() {
        OnBuffsChanged -= HandleBuffsChanged;

        ActiveBuffs.Clear();
    }

    public void HandleBuffsChanged(List<Buff> buffs) {
        // Check to see if we have been stunned or unstunned
        bool previousStunState = IsStunned;
        IsStunned = _myStats.IsStunImmune ? false : buffs.Exists(buff => buff.IsAStun);

        if (!previousStunState && IsStunned) {
            OnStunApplied?.Invoke();
        } else if (previousStunState && !IsStunned) {
            OnStunExpired?.Invoke();
        }
    }

    public bool HasBuff(string buffName) {
        return ActiveBuffs.Exists(buff => buff.Name.Equals(buffName));
    }

    public bool HasBuff(Buff buffToCheck) {
        return HasBuff(buffToCheck.Name);
    }

    public Buff GetBuff(string buffName) {
        return ActiveBuffs.Find(buff => buff.Name.Equals(buffName));
    }

    public Buff GetBuff(Buff buffPrefab) {
        return GetBuff(buffPrefab.Name);
    }

    private void Update() {
        List<Buff> expiredBuffs = new List<Buff>();
        ActiveBuffs.ForEach(buff => {
            if (buff == null) {
                return;
            }

            buff.UpdateElapsedTime(Time.deltaTime);

            if (buff.HasExpired()) {
                expiredBuffs.Add(buff);
            }
        });

        RemoveExpiredBuffs(expiredBuffs);
    }

    public void ApplyBuffTo(BuffController target, string buffName, float duration = -1f) {
        if (IsServer) {
            target.ServerApplyBuffFrom(this, buffName, duration);
        } else {
            CallServerApplyBuffTo(target, buffName, duration);
        }
    }

    [ServerRpc]
    private void CallServerApplyBuffTo(BuffController target, string buffName, float duration) {
        target.ServerApplyBuffFrom(this, buffName, duration);
    }

    private void ServerApplyBuffFrom(BuffController caster, string buffName, float duration = -1f) {
        if (!IsServer || !_canReceiveBuffs) {
            return;
        }

        Buff buffPrefab = ResourceManager.Instance.GetBuff(buffName);

        if (buffPrefab == null || (buffPrefab.IsAStun && _myStats.IsStunImmune)) {
            return;
        }

        /* TODO adding passed time here is actually a nerf to the player
         * because it removes the lagged time from the effect and the effect
         * only applies its changes on the server */

        CreateBuff(caster, buffName, duration, 0f);
        ObserversApplyBuffFrom(caster, buffName, duration, base.TimeManager.Tick);
    }

    private void CreateBuff(BuffController caster, string buffName, float duration, float passedTime, float elapsedTime = 0f, float addedTime = 0) {
        Buff buffPrefab = ResourceManager.Instance.GetBuff(buffName);

        Buff newBuff = Instantiate(buffPrefab);
        newBuff.Initailise(caster, _myStats, duration, elapsedTime, passedTime, addedTime);

        Buff originalBuff = GetBuff(newBuff.Name);
        if (originalBuff) {
            if (!originalBuff.ShouldBeOverriden(newBuff)) {
                return;
            }

            ActiveBuffs.RemoveAll(existingBuff => existingBuff.Name == newBuff.Name);
            originalBuff.RemoveEffects();
        }

        ActiveBuffs.Add(newBuff);
        newBuff.ApplyEffects();

        OnBuffsChanged?.Invoke(ActiveBuffs);
    }

    [ObserversRpc(ExcludeServer = true)]
    private void ObserversApplyBuffFrom(BuffController caster, string buffName, float duration, uint tick) {
        float passedTime = (float)base.TimeManager.TimePassed(tick, false);
        passedTime = Mathf.Min(MAX_PASSED_TIME, passedTime);

        CreateBuff(caster, buffName, duration, passedTime);
    }

    private void RemoveExpiredBuffs(List<Buff> expiredBuffs) {
        if (!IsServer) {
            return;
        }

        if (expiredBuffs.Count > 0) {
            expiredBuffs.ForEach(expiredBuff => {
                ServerRemoveBuff(expiredBuff.Name);
            });
        }
    }

    [Server(Logging = LoggingType.Off)]
    public void ServerRemoveBuff(string buffName) {
        DestroyBuff(buffName);
        ObserversRemoveBuff(buffName);
    }

    [ObserversRpc(ExcludeServer = true)]
    private void ObserversRemoveBuff(string buffName) {
        DestroyBuff(buffName);
    }

    private void DestroyBuff(string buffName) {
        Buff originalBuff = GetBuff(buffName);

        if (originalBuff == null) {
            return;
        }

        originalBuff.OnExpire();

        ActiveBuffs = ActiveBuffs.FindAll(buff => buff.Name != buffName).ToList();
        originalBuff.RemoveEffects();

        OnBuffsChanged?.Invoke(ActiveBuffs);
    }

    public void ServerUpdateBuffDuration(string buffName, float mod) {
        if (!IsServer || !HasBuff(buffName)) {
            return;
        }

        Buff currentBuff = GetBuff(buffName);
        currentBuff.AddedTime += mod == -1 ? currentBuff.MaxDuration : mod;
        OnBuffsChanged?.Invoke(ActiveBuffs);

        ObserversAddTimeToBuff(currentBuff.Caster, buffName, currentBuff.InitialDuration, currentBuff.ElapsedTime, currentBuff.AddedTime, base.TimeManager.Tick);
    }

    [ObserversRpc(ExcludeServer = true)]
    private void ObserversAddTimeToBuff(BuffController caster, string buffName, float initialDuration, float elapsedTime, float totalAddedTime, uint tick) {
        if (HasBuff(buffName)) {
            GetBuff(buffName).AddedTime = totalAddedTime;
            OnBuffsChanged?.Invoke(ActiveBuffs);
            return;
        }

        // if the buff doesnt exist, the server is saying it should have so correct that
        float passedTime = (float)base.TimeManager.TimePassed(tick, false);
        passedTime = Mathf.Min(MAX_PASSED_TIME, passedTime);

        CreateBuff(caster, buffName, 0f, passedTime, elapsedTime, totalAddedTime);
    }

    [Server(Logging = LoggingType.Off)]
    public bool ServerDispellBuffs(bool dispellPositiveBuffs, bool dispellNegativeBuffs, int numberToDispell = -1) {
        List<Buff> buffsToDispell = new List<Buff>();
        int remainingBuffsToDispell = numberToDispell == -1 ? 1000 : numberToDispell;
        bool dispelledABuff = false;

        ActiveBuffs.ForEach(buff => {
            if (remainingBuffsToDispell <= 0) {
                return;
            }

            if ((buff.IsPositive && dispellPositiveBuffs) || (!buff.IsPositive && dispellNegativeBuffs)) {
                buffsToDispell.Add(buff);
                remainingBuffsToDispell -= 1;
                dispelledABuff = true;
            }
        });

        buffsToDispell.ForEach(buff => {
            ServerRemoveBuff(buff.Name);
        });

        return dispelledABuff;
    }
}