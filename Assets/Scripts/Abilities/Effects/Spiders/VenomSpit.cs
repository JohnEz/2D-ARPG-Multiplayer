using System;
using System.Collections;
using UnityEngine;

public class VenomSpit : ProjectileHitEffect {
    private const int BASE_DAMAGE = 3;
    private const float POWER_SCALING = .1f;

    private const float ADDED_BUFF_DURATION = .75f;

    private const string BUFF = "Venom";

    protected override void HandleProjectileHit(Vector3 hitLocation, NetworkStats caster, NetworkStats hitCharacter) {
        BuffController hitBuffController = hitCharacter.GetComponent<BuffController>();
        BuffController casterBuffController = caster.GetComponent<BuffController>();

        if (hitBuffController.HasBuff(BUFF)) {
            hitBuffController.ServerUpdateBuffDuration(BUFF, ADDED_BUFF_DURATION);
        } else {
            casterBuffController.ApplyBuffTo(hitBuffController, BUFF, 1f);
        }

        caster.DealDamageTo(gameObject.name, hitCharacter, BASE_DAMAGE, POWER_SCALING);
    }
}