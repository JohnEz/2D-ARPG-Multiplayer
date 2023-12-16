using System.Collections;
using UnityEngine;

public class SlashAbility : AbilityEffect {
    public string slashId;

    public override void OnCastComplete(bool isOwner) {
        base.OnCastComplete(isOwner);

        if (isOwner) {
            _caster.GetComponent<SlashSpawner>().Slash(slashId, _caster.transform.position, _casterController.AimDirection);
        }

        Destroy(gameObject);
    }
}