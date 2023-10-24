using System.Collections;
using UnityEngine;

public class Frostbolt : ProjectileHitEffect {
    private const int BASE_DAMAGE = 15;

    private const int CHILL_DAMAGE = 2;

    protected override void HandleProjectileHit(Vector3 hitLocation, NetworkStats caster, NetworkStats hitCharacter) {
        int damage = BASE_DAMAGE;

        BuffController hitBuffController = hitCharacter.GetComponent<BuffController>();

        if (hitBuffController.HasBuff("Chill")) {
            hitBuffController.ServerUpdateBuffDuration("Chill", .8f);
            damage += CHILL_DAMAGE;
        }

        hitCharacter.TakeDamage(damage, caster.IsOwner, caster.GetComponent<CharacterController>());
    }
}