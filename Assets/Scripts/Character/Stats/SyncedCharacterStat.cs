using FishNet.Object.Synchronizing;
using FishNet.Object.Synchronizing.Internal;
using FishNet.Serializing;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Stat {
    public float BaseValue;
    public float ModifiedValue;

    public Stat(float baseValue, float modified) {
        BaseValue = baseValue;
        ModifiedValue = modified;
    }
}

//TODO add a type template for int or float?
[Serializable]
public class SyncedCharacterStat : SyncBase, ICustomSync {
    public Stat Value = new Stat();

    public float CurrentValue { get { return Value.ModifiedValue; } }

    public float BaseValue { get { return Value.BaseValue; } }

    private Stat _initialValue;

    [SerializeField]
    private bool _isInteger;

    private List<StatModifier> _statModifiers = new List<StatModifier>();

    public Action OnValueChanged;

    protected override void Registered() {
        base.Registered();
        _initialValue = Value;
    }

    public void SetBaseValue(float newValue) {
        if (Value.BaseValue == newValue) {
            return;
        }

        Value.BaseValue = newValue;
        UpdateCachedValue();
        OnValueChanged?.Invoke();
    }

    public void AddModifier(StatModifier mod) {
        _statModifiers.Add(mod);
        _statModifiers.Sort();
        UpdateCachedValue();
        OnValueChanged?.Invoke();
    }

    public bool RemoveModifier(StatModifier mod) {
        if (_statModifiers.Remove(mod)) {
            UpdateCachedValue();
            OnValueChanged?.Invoke();
            return true;
        }
        return false;
    }

    public bool RemoveAllModifiersFromSource(object source) {
        bool hasRemoved = false;

        int removed = _statModifiers.RemoveAll(mod => mod.Source == source);

        if (removed > 0) {
            UpdateCachedValue();
            hasRemoved = true;
            OnValueChanged?.Invoke();
        }

        return hasRemoved;
    }

    public void ForceUpdateCachedValue() {
        UpdateCachedValue();
    }

    protected virtual void UpdateCachedValue() {
        Value.ModifiedValue = CalculateFinalValue();
        Dirty();
    }

    protected float CalculateFinalValue() {
        float finalValue = Value.BaseValue;
        float sumPercentage = 0;

        for (int i = 0; i < _statModifiers.Count; i++) {
            StatModifier mod = _statModifiers[i];

            switch (mod.Type) {
                case StatModType.Flat:
                    finalValue += mod.Value;
                    break;

                case StatModType.PercentageAdd:
                    sumPercentage += mod.Value;

                    // if this is the last PercentageAdd, calculate the change
                    if (i + 1 >= _statModifiers.Count || _statModifiers[i + 1].Type != StatModType.PercentageAdd) {
                        finalValue *= 1 + sumPercentage;
                    }

                    break;

                case StatModType.Percentage:
                    finalValue *= 1 + mod.Value;
                    break;
            }
        }

        return (float)Math.Round(finalValue, _isInteger ? 0 : 4);
    }

    public override string ToString() => Value.ToString();

    public override void WriteDelta(PooledWriter writer, bool resetSyncTick = true) {
        base.WriteDelta(writer, resetSyncTick);

        writer.WriteSingle(Value.BaseValue);

        writer.WriteSingle(Value.ModifiedValue);
    }

    public override void WriteFull(PooledWriter writer) {
        WriteHeader(writer, false);

        writer.WriteSingle(Value.BaseValue);

        writer.WriteSingle(Value.ModifiedValue);
    }

    public override void Read(PooledReader reader, bool asServer) {
        float baseValue = reader.ReadSingle();
        float modifiedValue = reader.ReadSingle();

        bool discardChanges = !asServer && NetworkManager.IsServer;

        if (discardChanges) {
            return;
        }

        Value.BaseValue = baseValue;
        Value.ModifiedValue = modifiedValue;

        OnValueChanged?.Invoke();
    }

    public object GetSerializedType() => typeof(Stat);
}