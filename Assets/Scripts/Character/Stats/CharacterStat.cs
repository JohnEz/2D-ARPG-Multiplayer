using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO add a type template for int or float?
[Serializable]
public class CharacterStat {

    [SerializeField]
    protected float _baseValue;

    public float BaseValue {
        get => _baseValue;
        set => SetBaseValue(value);
    }

    [SerializeField]
    private bool _isInteger;

    protected bool isDirty = true;
    protected float _value;
    public float Value { get => GetValue(); }

    private List<StatModifier> _statModifiers;

    public Action OnValueChanged;

    public CharacterStat() {
        _statModifiers = new List<StatModifier>();
    }

    public CharacterStat(float baseValue, bool isInteger) : this() {
        BaseValue = baseValue;
        _isInteger = isInteger;
    }

    public void SetBaseValue(float value) {
        _baseValue = value;
        isDirty = true;
        OnValueChanged?.Invoke();
    }

    public void AddModifier(StatModifier mod) {
        _statModifiers.Add(mod);
        _statModifiers.Sort();
        isDirty = true;
        OnValueChanged?.Invoke();
    }

    public bool RemoveModifier(StatModifier mod) {
        if (_statModifiers.Remove(mod)) {
            isDirty = true;
            OnValueChanged?.Invoke();
            return true;
        }
        return false;
    }

    public bool RemoveAllModifiersFromSource(object source) {
        bool hasRemoved = false;

        int removed = _statModifiers.RemoveAll(mod => mod.Source == source);

        if (removed > 0) {
            isDirty = true;
            hasRemoved = true;
            OnValueChanged?.Invoke();
        }

        return hasRemoved;
    }

    private float GetValue() {
        if (isDirty) {
            UpdateCachedValue();
            isDirty = false;
        }
        return _value;
    }

    protected virtual void UpdateCachedValue() {
        _value = CalculateFinalValue();
    }

    protected float CalculateFinalValue() {
        float finalValue = BaseValue;
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
}