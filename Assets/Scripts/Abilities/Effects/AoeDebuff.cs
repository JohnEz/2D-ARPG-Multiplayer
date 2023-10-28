using FishNet;
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

    [SerializeField]
    private bool _UpdateDurationIfAlreadyPresent = false;

    public override void OnCastComplete(bool isOwner) {
        base.OnCastComplete(isOwner);

        if (InstanceFinder.IsServer) {
            Vector3 targetLocation = _caster.transform.position;
            NetworkStats casterStats = _caster.GetComponent<NetworkStats>();

            List<NetworkStats> hitTargets = PredictedTelegraph.GetCircleHitTargets(targetLocation, _radius, casterStats, false, true, false);

            hitTargets.ForEach(target => {
                BuffController targetBuffController = target.GetComponent<BuffController>();

                if (targetBuffController.HasBuff(_debuff) && _UpdateDurationIfAlreadyPresent) {
                    targetBuffController.ServerUpdateBuffDuration(_debuff, _duration);
                    return;
                }

                targetBuffController.ServerApplyBuff(_debuff, _duration);
            });
        }

        Destroy(gameObject);
    }
}