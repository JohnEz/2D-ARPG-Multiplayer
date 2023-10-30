using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAbility : AbilityEffect {
    public string projectileId;

    [SerializeField]
    private bool _targetsLocation = false;

    public override void OnCastComplete(bool isOwner) {
        base.OnCastComplete(isOwner);

        if (isOwner) {
            _caster.GetComponent<ProjectileSpawner>().Fire(projectileId, _caster.transform.position, _caster.AimLocation, _targetsLocation);
        }

        Destroy(gameObject);
    }
}