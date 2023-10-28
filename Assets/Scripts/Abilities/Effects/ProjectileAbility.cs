using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAbility : AbilityEffect {
    public string projectileId;

    public override void OnCastComplete(bool isOwner) {
        base.OnCastComplete(isOwner);

        if (isOwner) {
            _caster.GetComponent<ProjectileSpawner>().Fire(projectileId, _caster.transform.position, _caster.AimDirection);
        }

        Destroy(gameObject);
    }
}