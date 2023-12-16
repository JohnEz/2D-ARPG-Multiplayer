using System.Collections;
using UnityEngine;

public class TakeHealingEffect : BuffTickEffect {

    [SerializeField]
    private int BASE_HEALING = 2;

    [SerializeField]
    private const float POWER_SCALING = .2f;

    public override void OnTick(bool isOwner, NetworkStats target) {
        base.OnCastComplete(isOwner);

        _caster.GiveHealingTo(gameObject.name, target, BASE_HEALING, POWER_SCALING);

        Destroy(gameObject);
    }
}