using FishNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeHeal : AbilityEffect {

    [SerializeField]
    private float _radius = 2f;

    [SerializeField]
    private int _healAmount = 0;

    public override void OnCastComplete(bool isOwner) {
        base.OnCastComplete(isOwner);

        if (InstanceFinder.IsServer) {
            Vector3 targetLocation = _caster.transform.position;
            NetworkStats casterStats = _caster.GetComponent<NetworkStats>();

            List<NetworkStats> hitTargets = PredictedTelegraph.GetCircleHitTargets(targetLocation, _radius, casterStats, false, false, true);

            hitTargets.ForEach(target => {
                target.ReceiveHealing(_healAmount, casterStats.GetComponent<CharacterController>());
            });
        }

        Destroy(gameObject);
    }
}