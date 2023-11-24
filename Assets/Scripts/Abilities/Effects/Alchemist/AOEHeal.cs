using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEHeal : PersistentAOEHitEffect {

    [SerializeField]
    private int _healAmount = 6;

    protected override void HandleTick(NetworkStats caster, List<NetworkStats> hitTargets) {
        base.HandleTick(caster, hitTargets);

        hitTargets.ForEach(hitTarget => {
            hitTarget.ReceiveHealing(_healAmount, caster.GetComponent<CharacterController>());
        });
    }
}