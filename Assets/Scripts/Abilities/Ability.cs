using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public enum TargetAttribute {
    DISTANCE,
    CURRENT_HEALTH_PERCENT,
    ROLE
}

[System.Serializable]
public enum Quantifier {
    LT,
    GT,
    EQ,
    LTEQ,
    GTEQ,
}

[System.Serializable]
public struct AbilityTargetCondition {
    public TargetAttribute attribute;
    public Quantifier AttributeQuantifier;
    public float amount;
}

[System.Serializable]
public struct AiAbilityDetails {
    public float AbilityRange;
    public float ProjectileSpeed; // this should come from the projectile itself or be an aim ahead stat for telegraphs too
    public bool IsSupportAbility;
    public bool IsAutoCast;

    public AbilityTargetCondition targetConditions;
}

[CreateAssetMenu(fileName = "Ability", menuName = "2d RPG/New Ability")]
public class Ability : ScriptableObject {
    public string AbilityName;
    public Sprite Icon;

    public GameObject AbilityEffectPrefab;

    public float CastTime;
    public float Cooldown;
    private float _remainingCooldown;

    public AudioClip PrecastSFX;
    public GameObject PrecastVFX;

    public float SpeedWhileCasting = 0.1f;

    [SerializeField]
    private int _maxCharges = 1;

    public int MaxCharges { get { return _maxCharges; } }

    private int _currentCharges = 0;

    public int CurrentCharges { get { return _currentCharges; } }

    [HideInInspector]
    public float TimeCast = -Mathf.Infinity;

    [SerializeField]
    public List<TargetGraphic> TargetGraphics;

    [SerializeField]
    public AiAbilityDetails AiDetails;

    [SerializeField]
    public string Description;

    [SerializeField]
    public List<string> buffIds;

    [SerializeField]
    public bool _isConsumable = false;

    public void Awake() {
        _remainingCooldown = 0;
        _currentCharges = MaxCharges;
    }

    public void Update() {
        // consumables dont go on cooldown or regen charges
        if (_isConsumable) {
            return;
        }

        if (_remainingCooldown <= 0) {
            return;
        }

        _remainingCooldown -= Time.deltaTime;

        if (_remainingCooldown > 0) {
            return;
        }

        _currentCharges += 1;

        if (CurrentCharges == MaxCharges) {
            return;
        }

        _remainingCooldown = Cooldown;
    }

    public bool IsOnCooldown() {
        return _currentCharges == 0;
    }

    public bool CanCast() {
        return !IsOnCooldown();
    }

    public void OnCast() {
        if (!IsOnCooldown()) {
            _remainingCooldown = Cooldown;
        }

        _currentCharges -= 1;
    }

    public float GetRemainingCooldown() {
        return Mathf.Max(_remainingCooldown, 0);
    }

    public string GetCooldownAsString() {
        float remainingCooldown = GetRemainingCooldown();

        if (remainingCooldown <= 0) {
            return string.Empty;
        }

        return remainingCooldown > 1 ? remainingCooldown.ToString("F0") : remainingCooldown.ToString("F1");
    }
}