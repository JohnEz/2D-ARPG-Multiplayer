using FishNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Pathfinding.Util.RetainedGizmos;

public class AoeHeal : AbilityEffect {

    [SerializeField]
    private float _radius = 2f;

    [SerializeField]
    private int BASE_HEALING = 0;

    [SerializeField]
    private float POWER_SCALING = 0f;

    [SerializeField]
    private bool _canHitCaster = false;

    [SerializeField]
    private bool _canHitAlly = false;

    [SerializeField]
    private bool _canHitEnemy = false;

    [SerializeField]
    private GameObject _characterHitVfx;

    [SerializeField]
    private bool isTrueHealing = false;

    public override void OnCastComplete(bool isOwner) {
        base.OnCastComplete(isOwner);

        if (InstanceFinder.IsServer) {
            Vector3 targetLocation = _caster.transform.position;

            List<NetworkStats> hitTargets = PredictedTelegraph.GetCircleHitTargets(targetLocation, _radius, _caster, _canHitCaster, _canHitEnemy, _canHitAlly);

            hitTargets.ForEach(hitTarget => {
                _caster.GiveHealingTo(gameObject.name, hitTarget, BASE_HEALING, POWER_SCALING, isTrueHealing);

                if (_characterHitVfx) {
                    GameObject hitVFX = Instantiate(_characterHitVfx, hitTarget.transform);
                    hitVFX.transform.position = hitTarget.transform.position;
                }
            });
        }

        Destroy(gameObject);
    }
}