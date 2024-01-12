using System;
using System.Collections;
using UnityEngine;

public class ToxicBolt : ProjectileHitEffect {
    private const float POWER_SCALING = 1.3f;

    private const string BUFF = "Toxic";
    private const string BUFF2 = "Toxic II";
    private const string BUFF3 = "Toxic III";

    protected override void HandleProjectileHit(Vector3 hitLocation, NetworkStats caster, NetworkStats hitCharacter) {
        BuffController hitBuffController = hitCharacter.GetComponent<BuffController>();
        BuffController casterBuffController = caster.GetComponent<BuffController>();

        if (hitBuffController.HasBuff(BUFF3)) {
            hitBuffController.ServerUpdateBuffDuration(BUFF3, -1);
        } else if (hitBuffController.HasBuff(BUFF2)) {
            hitBuffController.ServerRemoveBuff(BUFF2);
            casterBuffController.ApplyBuffTo(hitBuffController, BUFF3);
        } else if (hitBuffController.HasBuff(BUFF)) {
            hitBuffController.ServerRemoveBuff(BUFF);
            casterBuffController.ApplyBuffTo(hitBuffController, BUFF2);
        } else {
            casterBuffController.ApplyBuffTo(hitBuffController, BUFF);
        }

        caster.DealDamageTo(gameObject.name, hitCharacter, POWER_SCALING);
    }
}