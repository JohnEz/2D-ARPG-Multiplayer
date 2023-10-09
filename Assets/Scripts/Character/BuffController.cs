using FishNet;
using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffController : NetworkBehaviour {
    private const float MAX_PASSED_TIME = 0.3f;

    private NetworkStats _myCharacter;

    public event Action<List<Buff>> OnBuffsChanged;

    private List<Buff> _activeBuffs;

    public List<Buff> ActiveBuffs {
        get { return _activeBuffs; }
        set { _activeBuffs = value; OnBuffsChanged?.Invoke(_activeBuffs); }
    }

    private void Awake() {
        _myCharacter = GetComponent<NetworkStats>();
        ActiveBuffs = new List<Buff>();
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
            buff.UpdateElapsedTime(Time.deltaTime);

            if (buff.HasExpired()) {
                expiredBuffs.Add(buff);
            }
        });

        RemoveExpiredBuffs(expiredBuffs);
    }

    public void ServerApplyBuff(string buffName) {
        if (!IsServer) {
            return;
        }

        CreateBuff(buffName, 0f);
        ObserversApplyBuff(buffName, base.TimeManager.Tick);
    }

    private void CreateBuff(string buffName, float passedTime, float elapsedTime = 0f, float addedTime = 0) {
        Buff buffPrefab = ResourceManager.Instance.GetBuff(buffName);

        Buff newBuff = Instantiate(buffPrefab);
        newBuff.Initailise(_myCharacter, elapsedTime, passedTime, addedTime);

        Buff originalBuff = GetBuff(newBuff.Name);
        if (originalBuff) {
            if (!originalBuff.ShouldBeOverriden(newBuff)) {
                return;
            }

            ActiveBuffs.RemoveAll(existingBuff => existingBuff.Name == newBuff.Name);
        }

        ActiveBuffs.Add(newBuff);

        OnBuffsChanged?.Invoke(ActiveBuffs);
    }

    [ObserversRpc(ExcludeServer = true)]
    private void ObserversApplyBuff(string buffName, uint tick) {
        float passedTime = (float)base.TimeManager.TimePassed(tick, false);
        passedTime = Mathf.Min(MAX_PASSED_TIME, passedTime);

        CreateBuff(buffName, passedTime);
    }

    private void RemoveExpiredBuffs(List<Buff> expiredBuffs) {
        if (!IsServer) {
            return;
        }

        if (expiredBuffs.Count > 0) {
            expiredBuffs.ForEach(expiredBuff => {
                expiredBuff.OnExpire();
                ServerRemoveBuff(expiredBuff.Name);
            });
        }
    }

    public void ServerRemoveBuff(string buffName) {
        if (!IsServer) {
            return;
        }

        DestroyBuff(buffName);
        ObserversRemoveBuff(buffName);
    }

    [ObserversRpc(ExcludeServer = true)]
    private void ObserversRemoveBuff(string buffName) {
        DestroyBuff(buffName);
    }

    private void DestroyBuff(string buffName) {
        ActiveBuffs = ActiveBuffs.FindAll(buff => buff.Name != buffName).ToList();
        OnBuffsChanged.Invoke(ActiveBuffs);
    }

    public void ServerUpdateBuffDuration(string buffName, float mod) {
        if (!IsServer || !HasBuff(buffName)) {
            return;
        }

        Buff currentBuff = GetBuff(buffName);
        currentBuff.AddedTime += mod;

        ObserversAddTimeToBuff(buffName, currentBuff.ElapsedTime, currentBuff.AddedTime, base.TimeManager.Tick);
    }

    [ObserversRpc(ExcludeServer = true)]
    private void ObserversAddTimeToBuff(string buffName, float elapsedTime, float totalAddedTime, uint tick) {
        if (HasBuff(buffName)) {
            GetBuff(buffName).AddedTime = totalAddedTime;
            OnBuffsChanged?.Invoke(ActiveBuffs);
            return;
        }

        // if the buff doesnt exist, the server is saying it should have so correct that
        float passedTime = (float)base.TimeManager.TimePassed(tick, false);
        passedTime = Mathf.Min(MAX_PASSED_TIME, passedTime);

        CreateBuff(buffName, passedTime, elapsedTime, totalAddedTime);
    }
}