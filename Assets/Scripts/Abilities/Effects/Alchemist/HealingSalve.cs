using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingSalve : ProjectileHitEffect {
    private const int HEALING_SALVE_HEAL_AMOUNT = 14;

    private const string BUFF = "Rejuvenation";

    private float _radius = 2f;

    protected override void HandleProjectileHitLocation(Vector3 hitLocation, NetworkStats caster) {
        base.HandleProjectileHitLocation(hitLocation, caster);

        if (caster == null) {
            return;
        }

        List<NetworkStats> hitTargets = PredictedTelegraph.GetCircleHitTargets(hitLocation, _radius, caster, true, false, true);

        hitTargets.ForEach(hitTarget => HandleCharacterHit(caster, hitTarget));
    }

    private void HandleCharacterHit(NetworkStats caster, NetworkStats hitTarget) {
        if (hitTarget == null) {
            return;
        }

        int baseHealAmount = HEALING_SALVE_HEAL_AMOUNT;

        BuffController hitBuffController = hitTarget.GetComponent<BuffController>();

        hitBuffController.ServerApplyBuff(BUFF);

        hitTarget.ReceiveHealing(baseHealAmount, caster.GetComponent<CharacterController>());
    }
}