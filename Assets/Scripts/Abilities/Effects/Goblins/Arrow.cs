using System.Collections;
using UnityEngine;

public class Arrow : ProjectileHitEffect {
    private const int BASE_DAMAGE = 8;

    protected override void HandleProjectileHit(Vector3 hitLocation, NetworkStats caster, NetworkStats hitCharacter) {
        int damage = BASE_DAMAGE;

        hitCharacter.TakeDamage(damage, caster.IsOwner, caster.GetComponent<CharacterController>());
    }
}