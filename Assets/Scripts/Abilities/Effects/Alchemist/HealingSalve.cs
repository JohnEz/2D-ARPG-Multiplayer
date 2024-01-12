using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingSalve : ProjectileHitEffect {
    private const float POWER_SCALING = 1.4f;

    private const string BUFF = "Rejuvenation";
    private const string BUFF2 = "Rejuvenation II";
    private const string BUFF3 = "Rejuvenation III";

    private float _radius = 2f;

    protected override void HandleProjectileHitLocation(Vector3 hitLocation, NetworkStats caster) {
        base.HandleProjectileHitLocation(hitLocation, caster);

        if (caster == null) {
            return;
        }

        List<NetworkStats> hitTargets = PredictedTelegraph.GetCircleHitTargets(hitLocation, _radius, caster, true, false, true);

        hitTargets.ForEach(hitTarget => HandleProjectileHit(hitLocation, caster, hitTarget));
    }

    protected override void HandleProjectileHit(Vector3 hitLocation, NetworkStats caster, NetworkStats hitTarget) {
        base.HandleProjectileHit(hitLocation, caster, hitTarget);

        if (hitTarget == null) {
            return;
        }

        BuffController hitBuffController = hitTarget.GetComponent<BuffController>();
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

        caster.GiveHealingTo(gameObject.name, hitTarget, POWER_SCALING);
    }
}