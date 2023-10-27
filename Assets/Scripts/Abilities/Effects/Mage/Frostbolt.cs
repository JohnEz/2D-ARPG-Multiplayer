using System.Collections;
using UnityEngine;

public class Frostbolt : ProjectileHitEffect {
    private const int BASE_DAMAGE = 15;

    private const int CHILL_DAMAGE = 2;

    private const float ADDED_CHILL_DURATION = .8f;

    protected override void HandleProjectileHit(Vector3 hitLocation, NetworkStats caster, NetworkStats hitCharacter) {
        int damage = BASE_DAMAGE;

        BuffController hitBuffController = hitCharacter.GetComponent<BuffController>();

        if (hitBuffController.HasBuff("Chill")) {
            hitBuffController.ServerUpdateBuffDuration("Chill", ADDED_CHILL_DURATION);
            damage += CHILL_DAMAGE;
        }

        hitCharacter.TakeDamage(damage, caster.IsOwner, caster.GetComponent<CharacterController>());
    }
}