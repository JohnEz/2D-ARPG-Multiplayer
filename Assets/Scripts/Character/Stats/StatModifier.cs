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
    public float Value;
    public StatModType Type;
    public object Source;
    public StatType Stat;

    public int Order { get => (int)Type; }

    public StatModifier(float value, StatModType type, object source = null) {
        Value = value;
        Type = type;
        Source = source;
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
            case StatType.HEALTH: return "Max Health";
            case StatType.POWER: return "Power";
            case StatType.MOVE_SPEED: return "Move Speed";
            default: return "";
        }
    }
}