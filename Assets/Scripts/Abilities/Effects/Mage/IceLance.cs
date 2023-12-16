using FishNet;
using System.Collections;
using UnityEngine;

public class IceLance : ProjectileHitEffect {
    private const int BASE_DAMAGE = 22;
    private const float POWER_SCALING = 1f;

    private const int CHILL_DAMAGE = 6;
    private const float CHILL_POWER_SCALING = 0.2f;

    private const float CHILL_DURATION = 1.5f;

    private const string DEBUFF = "Chill";

    protected override void HandleProjectileHit(Vector3 hitLocation, NetworkStats caster, NetworkStats hitCharacter) {
        int damage = BASE_DAMAGE;

        BuffController hitBuffController = hitCharacter.GetComponent<BuffController>();
        BuffController casterBuffController = caster.GetComponent<BuffController>();

        if (hitBuffController.HasBuff(DEBUFF)) {
            hitBuffController.ServerUpdateBuffDuration(DEBUFF, CHILL_DURATION);
            damage += caster.GetDamage(CHILL_DAMAGE, CHILL_POWER_SCALING);
        } else {
            casterBuffController.ApplyBuffTo(hitBuffController, DEBUFF, CHILL_DURATION);
        }

        caster.DealDamageTo(gameObject.name, hitCharacter, damage, POWER_SCALING);
    }
}