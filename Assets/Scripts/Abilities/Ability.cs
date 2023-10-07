using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ability {
    public GameObject abilityEffectPrefab;

    public float castTime;
    public float cooldown;

    [HideInInspector]
    public float timeCast = -Mathf.Infinity;

    public bool IsOnCooldown() {
        return timeCast + cooldown >= Time.time;
    }

    public bool CanCast() {
        return !IsOnCooldown();
    }

    public void OnCast() {
        timeCast = Time.time;
    }
}

public class AbilityEffect : MonoBehaviour {
    protected CharacterController _caster;

    public void Initialise(CharacterController caster) {
        _caster = caster;
    }

    public virtual void OnCastStart() {
    }

    public virtual void OnCastComplete() {
    }
}