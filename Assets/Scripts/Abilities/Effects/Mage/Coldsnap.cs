using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PredictedTelegraph))]
public class Coldsnap : TelegraphHitEffect {
    private const float POWER_SCALING = 1.6f;

    private const float CHILL_DURATION = 2.5f;

    private const string DEBUFF = "Chill";

    protected override void HandleCharacterHit(NetworkStats caster, NetworkStats hitCharacter) {
        base.HandleCharacterHit(caster, hitCharacter);

        BuffController hitBuffController = hitCharacter.GetComponent<BuffController>();
        BuffController casterBuffController = caster.GetComponent<BuffController>();

        caster.DealDamageTo(gameObject.name, hitCharacter, POWER_SCALING);

        if (hitBuffController.HasBuff(DEBUFF)) {
            hitBuffController.ServerRemoveBuff(DEBUFF);
            casterBuffController.ApplyBuffTo(hitBuffController, "Frozen");
        } else {
            casterBuffController.ApplyBuffTo(hitBuffController, DEBUFF, CHILL_DURATION);
        }
    }
}