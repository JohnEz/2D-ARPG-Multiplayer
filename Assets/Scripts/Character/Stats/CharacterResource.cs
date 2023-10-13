using FishNet.Serializing;
using System;
using UnityEngine;

[Serializable]
public class CharacterResource : CharacterStat {
    private float _currentValue;

    public float CurrentValue {
        get => GetCurrentValue();
        set => SetCurrentValue(value);
    }

    public CharacterResource() : base() {
        _currentValue = _baseValue;
    }

    public CharacterResource(float baseValue, bool isInteger) : base(baseValue, isInteger) {
        _currentValue = baseValue;
    }

    private float GetCurrentValue() {
        return _currentValue;
    }

    protected override void UpdateCachedValue() {
        float previousMax = _value;
        _value = CalculateFinalValue();

        float newMax = _value;

        if (previousMax > newMax) {
            _currentValue = Mathf.Min(newMax, CurrentValue);
        } else if (previousMax < newMax) {
            _currentValue += newMax - previousMax;
        }
    }

    private void SetCurrentValue(float newCurrent) {
        // Stop current value going above the max value
        float clampedCurrent = Mathf.Clamp(newCurrent, 0, Value);

        if (_currentValue != clampedCurrent) {
            _currentValue = clampedCurrent;
            OnValueChanged?.Invoke();
        }
    }

    public override string ToString() => $"{CurrentValue}/{Value}";
}