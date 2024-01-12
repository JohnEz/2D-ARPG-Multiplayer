using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet;
using FishNet.Managing.Logging;

public enum StatType {
    HEALTH,
    SHIELD,
    POWER,
    MOVE_SPEED,
    DAMAGE_TAKEN,
    HEALABLE_HEALTH,
}

public enum Faction {
    PLAYER,
    ENEMY,
    NEUTRAL,
}

public class NetworkStats : NetworkBehaviour {
    private BuffController _buffController;

    private const float BASE_HEALABLE_HEALTH = .33f;

    [SerializeField]
    public Faction Faction;

    [HideInInspector]
    [SyncVar(OnChange = nameof(HandleCurrentHealthChange), WritePermissions = WritePermission.ServerOnly)]
    private int _currentHealth = 100;

    public int CurrentHealth { get { return _currentHealth; } }

    [HideInInspector]
    [SyncVar(OnChange = nameof(HandleRemainingMaxHealthChange), WritePermissions = WritePermission.ServerOnly)]
    private int _remainingMaxHealth = 100;

    public int RemainingMaxHealth { get { return _remainingMaxHealth; } }

    // new Stats
    [SerializeField]
    private int _baseMaxHealth = 100;

    [SyncObject]
    public readonly SyncedCharacterStat MaxHealth = new SyncedCharacterStat();

    [SyncObject]
    public readonly SyncedCharacterStat HealableHealth = new SyncedCharacterStat();

    [SerializeField]
    private float _baseSpeed = 4f;

    [SyncObject]
    public readonly SyncedCharacterStat Speed = new SyncedCharacterStat();

    [SerializeField]
    private float _basePower = 0f;

    [SyncObject]
    public readonly SyncedCharacterStat Shield = new SyncedCharacterStat();

    [SyncObject]
    public readonly SyncedCharacterStat DamageTaken = new SyncedCharacterStat();

    [SyncObject]
    public readonly SyncedCharacterStat Power = new SyncedCharacterStat();

    public Dictionary<StatType, SyncedCharacterStat> StatList = new Dictionary<StatType, SyncedCharacterStat>();

    public override void OnStartServer() {
        MaxHealth.SetBaseValue(_baseMaxHealth);
        HealableHealth.SetBaseValue(BASE_HEALABLE_HEALTH);
        Power.SetBaseValue(_basePower);
        Speed.SetBaseValue(_baseSpeed);
        Shield.SetBaseValue(0f);
        DamageTaken.SetBaseValue(1f);

        _currentHealth = (int)MaxHealth.CurrentValue;
        _remainingMaxHealth = (int)MaxHealth.CurrentValue;
    }

    private void Start() {
        StatList.Add(StatType.HEALTH, MaxHealth);
        StatList.Add(StatType.POWER, Power);
        StatList.Add(StatType.MOVE_SPEED, Speed);
        StatList.Add(StatType.SHIELD, Shield);
        StatList.Add(StatType.DAMAGE_TAKEN, DamageTaken);
        StatList.Add(StatType.HEALABLE_HEALTH, HealableHealth);

        _buffController = GetComponent<BuffController>();
    }

    private void OnEnable() {
        MaxHealth.OnValueChanged += HandleMaxHealthChange;
        Shield.OnValueChanged += HandleCurrentShieldChange;
    }

    private void OnDisable() {
        MaxHealth.OnValueChanged -= HandleMaxHealthChange;
        Shield.OnValueChanged -= HandleCurrentShieldChange;
    }

    public event Action OnHealthDepleted;

    public event Action OnHealthReplenished;

    public event Action OnHealthChanged;

    public event Action<int, bool, NetworkStats> OnTakeDamage;

    public event Action<int, NetworkStats> OnReceiveHealing;

    #region Health Functions

    private void HandleMaxHealthChange() {
        OnHealthChanged?.Invoke();

        if (IsServer) {
            _currentHealth = Mathf.Min(_currentHealth, (int)MaxHealth.CurrentValue);
            // TODO do i need to change remaining max health here?
        }
    }

    private void HandleRemainingMaxHealthChange(int previousValue, int nextValue, bool asServer) {
        if (!asServer && InstanceFinder.IsHost) {
            // this is called for each client and for server, so host would call twice
            return;
        }

        OnHealthChanged?.Invoke();
    }

    private void HandleCurrentHealthChange(int previousValue, int nextValue, bool asServer) {
        if (!asServer && InstanceFinder.IsHost) {
            // this is called for each client and for server, so host would call twice
            return;
        }

        OnHealthChanged?.Invoke();
        HandleHitPointsChanged(previousValue, nextValue);
    }

    private void HandleCurrentShieldChange() {
        OnHealthChanged?.Invoke();
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

    [Server(Logging = LoggingType.Off)]
    public void TakeDamageServer(int damage) {
        int damageToTake = (int)(damage * DamageTaken.CurrentValue);
        //  we should always take at least 1 damage, even if we have 100% damage reduction
        damageToTake = Mathf.Max(damageToTake, 1);

        int remainingDamage = damageToTake;

        if (Shield.CurrentValue > 0) {
            // loop through all our buffs to find ones with shield mods
            List<Buff> shieldBuffs = _buffController.ActiveBuffs.FindAll(activeBuff => activeBuff.HasShield);

            // may need to order them by time applied or remaining time

            shieldBuffs.ForEach(buff => {
                if (remainingDamage < 0) {
                    return;
                }

                StatModifier shieldMod = buff.StatMods.Find(mod => mod.Stat == StatType.SHIELD);

                if (shieldMod == null || shieldMod.Value <= 0) {
                    return;
                }

                // reduce remaining shield and remaining damage
                if (shieldMod.Value >= remainingDamage) {
                    shieldMod.UpdateRemainingValue(-remainingDamage);
                    remainingDamage = 0;
                } else {
                    remainingDamage -= (int)shieldMod.Value;
                    shieldMod.UpdateRemainingValue(-shieldMod.Value);
                }
            });

            // make sure shield stat is recalculated
            Shield.ForceUpdateCachedValue();
        }

        SetHealthServer(_currentHealth - remainingDamage);
    }

    [Server(Logging = LoggingType.Off)]
    private void SetHealthServer(int health) {
        int newHealth = Math.Clamp(health, 0, _remainingMaxHealth);

        if (newHealth < _currentHealth) {
            _remainingMaxHealth = CalculateRemainingMaxHealth(newHealth);
        }

        _currentHealth = newHealth;
    }

    private int CalculateRemainingMaxHealth(int newHealth) {
        int healableHealth = (int)(MaxHealth.CurrentValue * HealableHealth.CurrentValue);

        if (newHealth >= _remainingMaxHealth - healableHealth) {
            return _remainingMaxHealth;
        }

        return Math.Clamp(newHealth + healableHealth, 0, (int)MaxHealth.CurrentValue);
    }

    public void DealDamageTo(string spellId, NetworkStats target, float powerScaling) {
        int damage = GetDamage(powerScaling);

        target.TakeDamage(spellId, this, damage);
    }

    //public void TakeDamage(int damage, CharacterController source) {
    public void TakeDamage(string spellId, NetworkStats source, int damage) {
        TakeDamageServer(damage);

        if (IsClient) {
            // TODO damage text needs more thinking as the value is calculated on the server
            // maybe i need an event for being hit and an event for taking numeric damage thats called in an observer? (not great for lag)
            int damageToTake = (int)(damage * DamageTaken.CurrentValue);

            OnTakeDamage?.Invoke(damage, false, source);
        }
    }

    [Server(Logging = LoggingType.Off)]
    public void ReceiveHealingServer(int healing, bool isTrueHealing) {
        if (isTrueHealing) {
            _remainingMaxHealth = Math.Clamp(_remainingMaxHealth + healing, 0, (int)MaxHealth.CurrentValue);
        }

        SetHealthServer(_currentHealth + healing);
    }

    public void GiveHealingTo(string spellId, NetworkStats target, float powerScaling, bool isTrueHealing = false) {
        int healing = GetHealing(powerScaling);

        target.ReceiveHealing(spellId, this, healing, isTrueHealing);
    }

    public void ReceiveHealing(string spellId, NetworkStats source, int healing, bool isTrueHealing) {
        ReceiveHealingServer(healing, isTrueHealing);

        if (InstanceFinder.IsClient) {
            OnReceiveHealing?.Invoke(healing, source);
        }
    }

    [Server(Logging = LoggingType.Off)]
    public void ServerKill() {
        _currentHealth = 0;
    }

    #endregion Health Functions

    #region Modifier functions

    public int GetPowerScaledValue(float powerScaling) {
        return (int)(powerScaling * Power.CurrentValue);
    }

    public int GetDamage(float powerScaling) {
        return GetPowerScaledValue(powerScaling);
    }

    public int GetHealing(float powerScaling) {
        return GetPowerScaledValue(powerScaling);
    }

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