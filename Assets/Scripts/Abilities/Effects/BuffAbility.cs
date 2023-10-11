using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffAbility : AbilityEffect {
    public string BuffId;

    public bool CanHitEnemies = false;
    public bool CanHitAllies = true;
    public bool CanHitSelf = true;

    public override void OnCastComplete() {
        NetworkStats hitTarget = null;

        List<NetworkStats> hitTargets = PredictedTelegraph.GetCircleHitTargets(_caster.AimLocation, .5f);

        if (hitTargets.Count > 0) {
            // if there were units near the mouse, pick the closest to the center
            // Todo needs to check faction
            hitTarget = GetClosestToPoint(_caster.AimLocation, hitTargets);
        }

        if (hitTarget == null && CanHitSelf) {
            hitTarget = _caster.GetComponent<NetworkStats>();
        }

        if (hitTarget != null) {
            hitTarget.GetComponent<BuffController>().ServerApplyBuff(BuffId);
        }

        Destroy(gameObject);
    }

    // Todo i feel this could go into a util section along with GetCircleHitTargets from the predictedTelegraph script
    private static NetworkStats GetClosestToPoint(Vector3 point, List<NetworkStats> hitTargets) {
        NetworkStats hitTarget = null;

        if (hitTargets.Count == 1) {
            hitTarget = hitTargets[0];
        } else {
            // find the closest to the center
            NetworkStats closestTarget = null;
            float closestDistance = Mathf.Infinity;

            hitTargets.ForEach((target) => {
                float distanceToCenter = Vector3.Distance(target.transform.position, point);

                if (distanceToCenter < closestDistance) {
                    closestDistance = distanceToCenter;
                    closestTarget = target;
                }
            });

            hitTarget = closestTarget;
        }

        return hitTarget;
    }
}