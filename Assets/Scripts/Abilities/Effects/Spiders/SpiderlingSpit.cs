using System.Collections;
using UnityEngine;

public class SpiderlingSpit : ProjectileHitEffect {
    private const float POWER_SCALING = 1f;

    protected override void HandleProjectileHit(Vector3 hitLocation, NetworkStats caster, NetworkStats hitCharacter) {
        caster.DealDamageTo(gameObject.name, hitCharacter, POWER_SCALING);
    }
}