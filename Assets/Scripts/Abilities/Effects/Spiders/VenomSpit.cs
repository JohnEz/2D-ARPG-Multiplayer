using System;
using System.Collections;
using UnityEngine;

public class VenomSpit : ProjectileHitEffect {
    private const float POWER_SCALING = .4f;

    private const float ADDED_BUFF_DURATION = 2f;

    private const string BUFF = "Venom";

    protected override void HandleProjectileHit(Vector3 hitLocation, NetworkStats caster, NetworkStats hitCharacter) {
        BuffController hitBuffController = hitCharacter.GetComponent<BuffController>();
        BuffController casterBuffController = caster.GetComponent<BuffController>();

        if (hitBuffController.HasBuff(BUFF)) {
            hitBuffController.ServerUpdateBuffDuration(BUFF, ADDED_BUFF_DURATION);
        } else {
            casterBuffController.ApplyBuffTo(hitBuffController, BUFF, ADDED_BUFF_DURATION);
        }

        caster.DealDamageTo(gameObject.name, hitCharacter, POWER_SCALING);
    }
}