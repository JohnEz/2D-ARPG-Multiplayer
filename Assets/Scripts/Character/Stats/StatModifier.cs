using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatModType {
    Flat,
    PercentageAdd,
    Percentage
}

[Serializable]
public class StatModifier : IComparable<StatModifier> {

    [SerializeField]
    private float _value;

    [SerializeField]
    private float powerScale = 0f;

    public StatModType Type;
    public object Source;
    public StatType Stat;

    public float Value { get { return _value; } }

    public int Order { get => (int)Type; }

    public StatModifier(float value, StatModType type, object source = null) {
        _value = value;
        Type = type;
        Source = source;
    }

    public void Initialise(int power) {
        if (Type == StatModType.Flat) {
            _value += power * powerScale;
        }
    }

    public void UpdateRemainingValue(float mod) {
        _value += mod;
    }

    public int CompareTo(StatModifier other) {
        return Order - other.Order;
    }

    public string ToTooltipString() {
        string statString = StatTypeToString(Stat);
        string sign = Value > 0 ? "+" : "-";
        string valueSuffix = Type != StatModType.Flat ? "%" : ""; // TODO might eventually need a switch for this
        string valueString = Mathf.Abs(Value).ToString(); // i want to control where the "-" is

        return $"{sign}{valueString}{valueSuffix} {statString}";
    }

    public static string StatTypeToString(StatType type) {
        switch (type) {
            case StatType.HEALTH:
            return "Max Health";

            case StatType.SHIELD:
            return "Shield";

            case StatType.POWER:
            return "Power";

            case StatType.MOVE_SPEED:
            return "Move Speed";

            default:
            return "";
        }
    }
}