using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAbility : AbilityEffect {
    public string projectileId;

    [SerializeField]
    private bool _targetsLocation = false;

    [SerializeField]
    private string telegraphId;

    public override void OnCastComplete(bool isOwner) {
        base.OnCastComplete(isOwner);

        if (isOwner) {
            _caster.GetComponent<ProjectileSpawner>().Fire(projectileId, _caster.transform.position, _casterController.AimLocation, _targetsLocation);

            if (telegraphId != null && telegraphId != "") {
                _caster.GetComponent<TelegraphSpawner>().Fire(telegraphId, _casterController.AimLocation);
            }
        }

        Destroy(gameObject);
    }
}