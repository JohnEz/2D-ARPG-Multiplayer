using FishNet;
using System.Collections;
using UnityEngine;

public class IceLance : ProjectileHitEffect {
    private const float POWER_SCALING = 2.1f;
    private const float CHILL_POWER_SCALING = 0.6f;

    private const float CHILL_DURATION = 1.5f;

    private const string DEBUFF = "Chill";

    protected override void HandleProjectileHit(Vector3 hitLocation, NetworkStats caster, NetworkStats hitCharacter) {
        float powerScaling = POWER_SCALING;

        BuffController hitBuffController = hitCharacter.GetComponent<BuffController>();
        BuffController casterBuffController = caster.GetComponent<BuffController>();

        if (hitBuffController.HasBuff(DEBUFF)) {
            hitBuffController.ServerUpdateBuffDuration(DEBUFF, CHILL_DURATION);
            powerScaling += CHILL_POWER_SCALING;
        } else {
            casterBuffController.ApplyBuffTo(hitBuffController, DEBUFF, CHILL_DURATION);
        }

        caster.DealDamageTo(gameObject.name, hitCharacter, powerScaling);
    }
}