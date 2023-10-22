using System.Collections;
using UnityEngine;

public class SlashAbility : AbilityEffect {
    public string slashId;

    public override void OnCastComplete(bool isOwner) {
        if (isOwner) {
            Vector3 directionToTarget = (new Vector3(_caster.AimLocation.x, _caster.AimLocation.y, 0) - _caster.transform.position).normalized;

            _caster.GetComponent<SlashSpawner>().Slash(slashId, _caster.transform.position, directionToTarget);
        }

        Destroy(gameObject);
    }
}