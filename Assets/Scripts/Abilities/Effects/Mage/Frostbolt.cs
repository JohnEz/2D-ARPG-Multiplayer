using System.Collections;
using UnityEngine;

public class Frostbolt : ProjectileHitEffect {
    private const float POWER_SCALING = 1.5f;

    private const float CHILL_POWER_SCALING = 0.3f;

    private const float ADDED_CHILL_DURATION = .8f;

    protected override void HandleProjectileHit(Vector3 hitLocation, NetworkStats caster, NetworkStats hitCharacter) {
        float damageScaling = POWER_SCALING;

        BuffController hitBuffController = hitCharacter.GetComponent<BuffController>();

        if (hitBuffController.HasBuff("Chill")) {
            hitBuffController.ServerUpdateBuffDuration("Chill", ADDED_CHILL_DURATION);
            damageScaling += CHILL_POWER_SCALING;
        }

        caster.DealDamageTo(gameObject.name, hitCharacter, damageScaling);
    }
}