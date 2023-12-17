using System.Collections;
using UnityEngine;

public class Frostbolt : ProjectileHitEffect {
    private const int BASE_DAMAGE = 10;
    private const float POWER_SCALING = .5f;

    private const int CHILL_DAMAGE = 1;
    private const float CHILL_POWER_SCALING = 0.2f;

    private const float ADDED_CHILL_DURATION = .8f;

    protected override void HandleProjectileHit(Vector3 hitLocation, NetworkStats caster, NetworkStats hitCharacter) {
        int damage = BASE_DAMAGE;

        BuffController hitBuffController = hitCharacter.GetComponent<BuffController>();

        if (hitBuffController.HasBuff("Chill")) {
            hitBuffController.ServerUpdateBuffDuration("Chill", ADDED_CHILL_DURATION);
            damage += caster.GetDamage(CHILL_DAMAGE, CHILL_POWER_SCALING);
        }

        caster.DealDamageTo(gameObject.name, hitCharacter, damage, POWER_SCALING);
    }
}