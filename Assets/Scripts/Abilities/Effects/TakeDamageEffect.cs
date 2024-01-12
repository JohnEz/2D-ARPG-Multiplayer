using System.Collections;
using UnityEngine;
using static Pathfinding.Util.RetainedGizmos;

public class TakeDamageEffect : BuffTickEffect {

    [SerializeField]
    private float POWER_SCALING = 0.2f;

    public override void OnTick(bool isOwner, NetworkStats target) {
        base.OnCastComplete(isOwner);

        _caster.DealDamageTo(gameObject.name, target, POWER_SCALING);

        Destroy(gameObject);
    }
}