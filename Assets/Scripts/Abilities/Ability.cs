using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public struct AiAbilityDetails {
    public float AbilityRange;
    public float ProjectileSpeed; // this should come from the projectile itself or be an aim ahead stat for telegraphs too
    public bool IsSupportAbility;
    public bool IsAutoCast;
}

[CreateAssetMenu(fileName = "Ability", menuName = "2d RPG/New Ability")]
public class Ability : ScriptableObject {
    public string AbilityName;
    public Sprite Icon;

    public GameObject AbilityEffectPrefab;

    public float CastTime;
    public float Cooldown;
    private float _remainingCooldown;

    public AudioClip CastSFX;
    public GameObject CastVFX;

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

    public void Awake() {
        _remainingCooldown = Cooldown;
    }

    public void Update() {
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
        return remainingCooldown > 1 ? remainingCooldown.ToString("F0") : remainingCooldown.ToString("F1");
    }
}