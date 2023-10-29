using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffAbility : AbilityEffect {
    public string BuffId;

    public bool CanHitEnemies = false;
    public bool CanHitAllies = true;
    public bool CanHitCaster = true;

    public static List<NetworkStats> GetHitTargets(NetworkStats casterStats, bool canHitCaster, bool canHitEnemies, bool canHitAllies) {
        List<NetworkStats> hitTargets = new List<NetworkStats>();

        GameObject.FindGameObjectsWithTag("Unit").ToList().ForEach((unit) => {
            NetworkStats unitStats = unit.GetComponent<NetworkStats>();

            if (unitStats == null) {
                return;
            }

            if (canHitCaster && unitStats == casterStats) {
                hitTargets.Add(unitStats);
            } else if (canHitEnemies && unitStats.Faction != casterStats.Faction) {
                hitTargets.Add(unitStats);
            } else if (canHitAllies && unitStats.Faction == casterStats.Faction) {
                hitTargets.Add(unitStats);
            }
        });

        return hitTargets;
    }

    public override void OnCastComplete(bool isOwner) {
        base.OnCastComplete(isOwner);

        if (isOwner) {
            NetworkStats hitTarget = null;
            BuffController casterBuffs = _caster.GetComponent<BuffController>();
            NetworkStats casterStats = _caster.GetComponent<NetworkStats>();

            List<NetworkStats> hitTargets = GetHitTargets(casterStats, CanHitCaster, CanHitEnemies, CanHitAllies);

            if (hitTargets.Count > 0) {
                // if there were units near the mouse, pick the closest to the center
                hitTarget = GetClosestToPoint(_caster.AimLocation, hitTargets);
            }

            if (hitTarget != null) {
                BuffController targetBuffs = hitTarget.GetComponent<BuffController>();

                casterBuffs.ApplyBuff(targetBuffs, BuffId);
            }
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