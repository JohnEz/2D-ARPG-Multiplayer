using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PredictedTelegraph))]
public class Coldsnap : TelegraphHitEffect {
    private const int BASE_DAMAGE = 16;
    private const float POWER_SCALING = 0.25f;

    private const float CHILL_DURATION = 2.5f;

    private const string DEBUFF = "Chill";

    protected override void HandleCharacterHit(NetworkStats caster, NetworkStats hitCharacter) {
        base.HandleCharacterHit(caster, hitCharacter);

        BuffController hitBuffController = hitCharacter.GetComponent<BuffController>();
        BuffController casterBuffController = caster.GetComponent<BuffController>();

        caster.DealDamageTo(gameObject.name, hitCharacter, BASE_DAMAGE, POWER_SCALING);

        if (hitBuffController.HasBuff(DEBUFF)) {
            hitBuffController.ServerRemoveBuff(DEBUFF);
            casterBuffController.ApplyBuffTo(hitBuffController, "Frozen");
        } else {
            casterBuffController.ApplyBuffTo(hitBuffController, DEBUFF, CHILL_DURATION);
        }
    }
}