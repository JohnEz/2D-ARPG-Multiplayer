﻿using FishNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Pathfinding.Util.RetainedGizmos;

public class ChallengingShout : AbilityEffect {
    private float _radius = 5f;

    private const float AGGRO_POWER_SCALING = 2.4f;

    private const float DAMAGE_POWER_SCALING = .6f;

    private float _baseValiantDuration = 1f;

    private float _valiantDurationPerHit = .75f;

    private string BUFF = "Valiant";

    public override void OnCastComplete(bool isOwner) {
        base.OnCastComplete(isOwner);

        Vector3 targetLocation = _caster.transform.position;

        List<NetworkStats> hitTargets = PredictedTelegraph.GetCircleHitTargets(targetLocation, _radius, _caster, false, true, false);

        int scaledAdditionalAggro = _caster.GetPowerScaledValue(AGGRO_POWER_SCALING);

        hitTargets.ForEach(hitTarget => {
            _caster.DealDamageTo(gameObject.name, hitTarget, DAMAGE_POWER_SCALING);

            if (InstanceFinder.IsServer) {
                AiBrain aiBrain = hitTarget.GetComponent<AiBrain>();
                if (aiBrain != null) {
                    aiBrain.TauntedServer(_casterController, scaledAdditionalAggro);
                }
            }
        });

        BuffController casterBuffController = _caster.GetComponent<BuffController>();

        float valiantDuration = _baseValiantDuration;

        if (hitTargets.Count > 0) {
            valiantDuration += _valiantDurationPerHit * hitTargets.Count;
        }

        if (casterBuffController.HasBuff(BUFF)) {
            casterBuffController.ServerUpdateBuffDuration(BUFF, valiantDuration);
        } else {
            casterBuffController.ApplyBuffTo(casterBuffController, BUFF);
        }
    }
}