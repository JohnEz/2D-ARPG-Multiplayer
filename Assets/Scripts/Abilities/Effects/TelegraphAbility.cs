using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelegraphAbility : AbilityEffect {
    public string telegraphId;

    public override void OnCastComplete() {
        _caster.GetComponent<TelegraphSpawner>().Fire(telegraphId, _caster.AimLocation);

        Destroy(gameObject);
    }
}