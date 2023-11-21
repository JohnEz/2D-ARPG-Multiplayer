using System;
using System.Collections;
using UnityEngine;

public class ToxicBolt : ProjectileHitEffect {
    private const int BASE_DAMAGE = 12;

    private const string BUFF = "Toxic";
    private const string BUFF2 = "Toxic II";
    private const string BUFF3 = "Toxic III";

    protected override void HandleProjectileHit(Vector3 hitLocation, NetworkStats caster, NetworkStats hitCharacter) {
        int damage = BASE_DAMAGE;

        BuffController hitBuffController = hitCharacter.GetComponent<BuffController>();

        if (hitBuffController.HasBuff(BUFF3)) {
            hitBuffController.ServerUpdateBuffDuration(BUFF3, -1);
        } else if (hitBuffController.HasBuff(BUFF2)) {
            hitBuffController.ServerRemoveBuff(BUFF2);
            hitBuffController.ServerApplyBuff(BUFF3);
        } else if (hitBuffController.HasBuff(BUFF)) {
            hitBuffController.ServerRemoveBuff(BUFF);
            hitBuffController.ServerApplyBuff(BUFF2);
        } else {
            hitBuffController.ServerApplyBuff(BUFF);
        }

        hitCharacter.TakeDamage(damage, caster.GetComponent<CharacterController>());
    }
}