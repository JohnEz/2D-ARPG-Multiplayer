﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingGoo : PersistentAOEHitEffect {

    [SerializeField]
    private const int BASE_HEALING = 6;

    [SerializeField]
    private const float POWER_SCALING = 0.6f;

    protected override void HandleTick(NetworkStats caster, List<NetworkStats> hitTargets) {
        base.HandleTick(caster, hitTargets);

        hitTargets.ForEach(hitTarget => {
            caster.GiveHealingTo(gameObject.name, hitTarget, BASE_HEALING, POWER_SCALING);
        });
    }
}