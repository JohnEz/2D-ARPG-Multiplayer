using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelegraphAbility : AbilityEffect {
    public string telegraphId;

    public override void OnCastComplete(bool isOwner) {
        base.OnCastComplete(isOwner);

        if (isOwner) {
            _caster.GetComponent<TelegraphSpawner>().Fire(telegraphId, _casterController.AimLocation);
        }

        Destroy(gameObject);
    }
}