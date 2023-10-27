using FishNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengingShout : AbilityEffect {
    private float _radius = 5f;

    private int _additionalAggro = 14;

    private int _damage = 6;

    private float _baseValiantDuration = 1f;

    private float _valiantDurationPerHit = .75f;

    public override void OnCastComplete(bool isOwner) {
        base.OnCastComplete(isOwner);

        Vector3 targetLocation = _caster.transform.position;
        NetworkStats casterStats = _caster.GetComponent<NetworkStats>();
        List<NetworkStats> hitTargets = PredictedTelegraph.GetCircleHitTargets(targetLocation, _radius, casterStats, false, true, false);

        hitTargets.ForEach(target => {
            target.TakeDamage(_damage, isOwner, _caster);

            if (InstanceFinder.IsServer) {
                AiBrain aiBrain = target.GetComponent<AiBrain>();
                if (aiBrain != null) {
                    aiBrain.TauntedServer(_caster, _additionalAggro);
                }
            }
        });

        if (isOwner) {
            BuffController buffController = _caster.GetComponent<BuffController>();

            float valiantDuration = _baseValiantDuration;

            if (hitTargets.Count > 0) {
                valiantDuration += _valiantDurationPerHit * hitTargets.Count;
            }

            buffController.ApplyBuff(buffController, "Valiant", valiantDuration);
        }
    }
}