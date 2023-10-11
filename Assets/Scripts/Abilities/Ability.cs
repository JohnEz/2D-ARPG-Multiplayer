using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "2d RPG/New Ability")]
public class Ability : ScriptableObject {
    public string AbilityName;
    public Sprite Icon;

    public GameObject AbilityEffectPrefab;

    public float CastTime;
    public float Cooldown;

    public AudioClip CastSFX;
    public GameObject CastVFX;

    public float SpeedWhileCasting = 0.1f;

    [HideInInspector]
    public float TimeCast = -Mathf.Infinity;

    [SerializeField]
    public List<TargetGraphic> TargetGraphics;

    public bool IsOnCooldown() {
        return TimeCast + Cooldown >= Time.time;
    }

    public bool CanCast() {
        return !IsOnCooldown();
    }

    public void OnCast() {
        TimeCast = Time.time;
    }

    public float GetRemainingCooldown() {
        float remainingCooldown = TimeCast + Cooldown - Time.time;

        return Mathf.Max(remainingCooldown, 0);
    }

    public string GetCooldownAsString() {
        float remainingCooldown = GetRemainingCooldown();
        return remainingCooldown > 1 ? remainingCooldown.ToString("F0") : remainingCooldown.ToString("F1");
    }
}