using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAbility : AbilityEffect {
    public string projectileId;

    public override void OnCastComplete(bool isOwner) {
        if (isOwner) {
            Vector3 directionToTarget = (new Vector3(_caster.AimLocation.x, _caster.AimLocation.y, 0) - _caster.transform.position).normalized;

            _caster.GetComponent<ProjectileSpawner>().Fire(projectileId, _caster.transform.position, directionToTarget);
        }

        Destroy(gameObject);
    }
}