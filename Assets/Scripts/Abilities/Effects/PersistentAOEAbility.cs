using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentAOEAbility : AbilityEffect {
    public string persistentId;

    public override void OnCastComplete(bool isOwner) {
        base.OnCastComplete(isOwner);

        if (isOwner) {
            _caster.GetComponent<AbilitySpawner>().CreatePersistentAOE(persistentId, _caster.AimLocation);
        }

        Destroy(gameObject);
    }
}