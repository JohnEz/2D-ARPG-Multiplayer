using System.Collections;
using UnityEngine;
using static Pathfinding.Util.RetainedGizmos;

public class TakeDamageEffect : BuffTickEffect {

    [SerializeField]
    private int BASE_DAMAGE = 2;

    [SerializeField]
    private const float POWER_SCALING = 0.2f;

    public override void OnTick(bool isOwner, NetworkStats target) {
        base.OnCastComplete(isOwner);

        _caster.DealDamageTo(gameObject.name, target, BASE_DAMAGE, POWER_SCALING);

        Destroy(gameObject);
    }
}