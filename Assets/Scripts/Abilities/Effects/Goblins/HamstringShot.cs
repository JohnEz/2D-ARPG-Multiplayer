using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HamstringShot : ProjectileHitEffect {
    private const int BASE_DAMAGE = 15;

    protected override void HandleProjectileHit(Vector3 hitLocation, NetworkStats caster, NetworkStats hitCharacter) {
        int damage = BASE_DAMAGE;

        BuffController hitBuffController = hitCharacter.GetComponent<BuffController>();

        hitBuffController.ServerApplyBuff("Snare");

        hitCharacter.TakeDamage(damage, caster.GetComponent<CharacterController>());
    }
}