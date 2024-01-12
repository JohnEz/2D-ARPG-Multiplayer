using System.Collections;
using UnityEngine;

public class Arrow : ProjectileHitEffect {
    private const float POWER_SCALING = 1.4f;

    protected override void HandleProjectileHit(Vector3 hitLocation, NetworkStats caster, NetworkStats hitCharacter) {
        caster.DealDamageTo(gameObject.name, hitCharacter, POWER_SCALING);
    }
}