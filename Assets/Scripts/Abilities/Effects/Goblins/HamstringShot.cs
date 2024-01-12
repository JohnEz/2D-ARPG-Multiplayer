using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HamstringShot : ProjectileHitEffect {
    private const float POWER_SCALING = 2f;

    protected override void HandleProjectileHit(Vector3 hitLocation, NetworkStats caster, NetworkStats hitCharacter) {
        BuffController hitBuffController = hitCharacter.GetComponent<BuffController>();
        BuffController casterBuffController = caster.GetComponent<BuffController>();

        casterBuffController.ApplyBuffTo(hitBuffController, "Snare");

        caster.DealDamageTo(gameObject.name, hitCharacter, POWER_SCALING);
    }
}