using System.Collections;
using UnityEngine;

public class Arrow : ProjectileHitEffect {
    private const int BASE_DAMAGE = 14;
    private const float POWER_SCALING = 1f;

    protected override void HandleProjectileHit(Vector3 hitLocation, NetworkStats caster, NetworkStats hitCharacter) {
        caster.DealDamageTo(gameObject.name, hitCharacter, BASE_DAMAGE, POWER_SCALING);
    }
}