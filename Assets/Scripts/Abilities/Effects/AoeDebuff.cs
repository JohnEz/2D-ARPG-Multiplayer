using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeDebuff : AbilityEffect {

    [SerializeField]
    private string _debuff = "Chill";

    [SerializeField]
    private float _radius = 2f;

    [SerializeField]
    private float _duration = 1.5f;

    public override void OnCastComplete(bool isOwner) {
        base.OnCastComplete(isOwner);

        if (isOwner) {
            Vector3 targetLocation = _caster.transform.position;
            NetworkStats casterStats = _caster.GetComponent<NetworkStats>();

            List<NetworkStats> hitTargets = PredictedTelegraph.GetCircleHitTargets(targetLocation, _radius, casterStats, false, true, false);

            hitTargets.ForEach(target => {
                BuffController targetBuffController = target.GetComponent<BuffController>();
                _caster.GetComponent<BuffController>().ApplyBuff(targetBuffController, _debuff, _duration);
            });
        }

        Destroy(gameObject);
    }
}