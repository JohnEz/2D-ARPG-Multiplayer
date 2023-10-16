using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO does this need to be its own class?
// this could be an aoe debuff with no hit time centered on caster
public class IceBarrierBreak : AbilityEffect {
    private const string DEBUFF = "Chill";

    private const float RADIUS = 2f;

    public override void OnCastComplete() {
        Vector3 targetLocation = _caster.transform.position;

        List<NetworkStats> hitTargets = PredictedTelegraph.GetCircleHitTargets(targetLocation, RADIUS);

        hitTargets.ForEach(target => {
            BuffController targetBuffController = target.GetComponent<BuffController>();
            _caster.GetComponent<BuffController>().ApplyBuff(targetBuffController, DEBUFF);
        });

        Destroy(gameObject);
    }
}