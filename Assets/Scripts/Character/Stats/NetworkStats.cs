using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet;

public enum StatType {
    HEALTH,
    POWER,
    MOVE_SPEED,
}

public class NetworkStats : NetworkBehaviour {

    [HideInInspector]
    [SyncVar(OnChange = nameof(HandleCurrentHealthChange), WritePermissions = WritePermission.ServerOnly)]
    private int _currentHealth = 100;

    public int CurrentHealth { get { return _currentHealth; } }

    //[SyncVar(OnChange = nameof(HandleMaxHealthChange), WritePermissions = WritePermission.ServerOnly)]
    //private int _maxHealth = 100;

    //public int MaxHealth { get { return _maxHealth; } }

    [SerializeField]
    private float _baseMoveSpeed = 4f;

    public float MoveSpeed { get { return _baseMoveSpeed; } }

    // new Stats
    [SerializeField]
    private int _baseMaxHealth = 100;

    [SyncObject]
    public readonly SyncedCharacterStat MaxHealth = new SyncedCharacterStat();

    [SerializeField]
    private float _baseSpeed = 4f;

    [SyncObject]
    public readonly SyncedCharacterStat Speed = new SyncedCharacterStat();

    [SerializeField]
    private float _basePower = 0f;

    [SyncObject]
    public readonly SyncedCharacterStat Power = new SyncedCharacterStat();

    public Dictionary<StatType, SyncedCharacterStat> StatList = new Dictionary<StatType, SyncedCharacterStat>();

    public override void OnStartServer() {
        MaxHealth.SetBaseValue(_baseMaxHealth);
        Power.SetBaseValue(_basePower);
        Speed.SetBaseValue(_baseSpeed);

        _currentHealth = (int)MaxHealth.CurrentValue;
    }

    private void Start() {
        StatList.Add(StatType.HEALTH, MaxHealth);
        StatList.Add(StatType.POWER, Power);
        StatList.Add(StatType.MOVE_SPEED, Speed);
    }

    private void OnEnable() {
        MaxHealth.OnValueChanged += HandleMaxHealthChange;
    }

    private void OnDisable() {
        MaxHealth.OnValueChanged -= HandleMaxHealthChange;
    }

    public event Action OnHealthDepleted;

    public event Action OnHealthReplenished;

    public event Action OnHealthChanged;

    public event Action<int, bool, bool> OnTakeDamage;

    public event Action<int> OnReceiveHealing;

    #region Health Functions

    private void HandleMaxHealthChange() {
        OnHealthChanged?.Invoke();

        _currentHealth = Mathf.Min(_currentHealth, (int)MaxHealth.CurrentValue);
    }

    private void HandleCurrentHealthChange(int previousValue, int nextValue, bool asServer) {
        if (!asServer && InstanceFinder.IsHost) {
            // this is called for each client and for server, so host would call twice
            return;
        }

        OnHealthChanged?.Invoke();
        HandleHitPointsChanged(previousValue, nextValue);
    }

    private void HandleMaxHealthChange(int previousValue, int nextValue, bool asServer) {
        if (!asServer && InstanceFinder.IsHost) {
            // this is called for each client and for server, so host would call twice
            return;
        }

        OnHealthChanged?.Invoke();
    }

    private void HandleHitPointsChanged(int previousValue, int nextValue) {
        if (previousValue > 0 && nextValue <= 0) {
            // newly reached 0 HP
            OnHealthDepleted?.Invoke();
            //if (InstanceFinder.IsServer) {
            //    NetworkObject.Despawn();
            //}
        } else if (previousValue <= 0 && nextValue > 0) {
            // newly revived
            OnHealthReplenished?.Invoke();
        }
    }

    [Server]
    public void TakeDamageServer(int damage) {
        _currentHealth = Math.Clamp(_currentHealth - damage, 0, (int)MaxHealth.CurrentValue);
    }

    public void TakeDamage(int damage, bool sourceIsPlayer) {
        if (IsServer) {
            TakeDamageServer(damage);
        }

        if (IsClient) {
            // TODO damage text needs more thinking as the value is calculated on the server
            // maybe i need an event for being hit and an event for taking numeric damage thats called in an observer? (not great for lag)
            OnTakeDamage.Invoke(damage, false, sourceIsPlayer);
        }
    }

    [Server]
    public void ReceiveHealingServer(int healing) {
        _currentHealth = Math.Clamp(_currentHealth + healing, 0, (int)MaxHealth.CurrentValue);
    }

    public void ReceiveHealing(int healing) {
        ReceiveHealingServer(healing);

        if (InstanceFinder.IsClient) {
            OnReceiveHealing.Invoke(healing);
        }
    }

    #endregion Health Functions

    #region Modifier functions

    public SyncedCharacterStat GetCharacterStat(StatType stat) {
        return StatList[stat];
    }

    public void ApplyStatMods(List<StatModifier> mods) {
        mods.ForEach(mod => ApplyStatMod(mod));
    }

    public void ApplyStatMod(StatModifier mod) {
        SyncedCharacterStat statToMod = GetCharacterStat(mod.Stat);
        statToMod.AddModifier(mod);
    }

    public void RemoveStatMods(List<StatModifier> mods) {
        mods.ForEach(mod => RemoveStatMod(mod));
    }

    public void RemoveStatMod(StatModifier mod) {
        SyncedCharacterStat statToMod = GetCharacterStat(mod.Stat);
        statToMod.RemoveModifier(mod);
    }

    public void RemoveAllStatModsBySource(object source) {
        foreach (SyncedCharacterStat stat in StatList.Values) {
            stat.RemoveAllModifiersFromSource(source);
        }
    }

    #endregion Modifier functions
}