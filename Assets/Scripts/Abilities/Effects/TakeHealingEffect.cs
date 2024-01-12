using System.Collections;
using UnityEngine;

public class TakeHealingEffect : BuffTickEffect {

    [SerializeField]
    private float POWER_SCALING = .2f;

    public override void OnTick(bool isOwner, NetworkStats target) {
        base.OnCastComplete(isOwner);

        _caster.GiveHealingTo(gameObject.name, target, POWER_SCALING);

        Destroy(gameObject);
    }
}