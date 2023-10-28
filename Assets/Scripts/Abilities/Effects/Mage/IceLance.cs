using FishNet;
using System.Collections;
using UnityEngine;

public class IceLance : ProjectileHitEffect {
    private const int BASE_DAMAGE = 22;

    private const int CHILL_DAMAGE = 6;

    private const float CHILL_DURATION = 1.5f;

    private const string DEBUFF = "Chill";

    protected override void HandleProjectileHit(Vector3 hitLocation, NetworkStats caster, NetworkStats hitCharacter) {
        int damage = BASE_DAMAGE;

        BuffController hitBuffController = hitCharacter.GetComponent<BuffController>();

        if (hitBuffController.HasBuff(DEBUFF)) {
            damage += CHILL_DAMAGE;
            hitBuffController.ServerUpdateBuffDuration(DEBUFF, CHILL_DURATION);
        } else {
            hitBuffController.ServerApplyBuff(DEBUFF, CHILL_DURATION);
        }

        hitCharacter.TakeDamage(damage, caster.IsOwner, caster.GetComponent<CharacterController>());
    }
}