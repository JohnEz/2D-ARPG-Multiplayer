﻿using System.Collections;
using UnityEngine;

public class GoblinSlash : MonoBehaviour {
    private const float POWER_SCALING = 1f;

    private void OnEnable() {
        GetComponent<PredictedProjectile>().OnHit += HandleCharacterHit;
    }

    private void OnDisable() {
        GetComponent<PredictedProjectile>().OnHit -= HandleCharacterHit;
    }

    private void HandleCharacterHit(Vector3 Location, NetworkStats caster, NetworkStats hitCharacter) {
        caster.DealDamageTo(gameObject.name, hitCharacter, POWER_SCALING);
    }
}