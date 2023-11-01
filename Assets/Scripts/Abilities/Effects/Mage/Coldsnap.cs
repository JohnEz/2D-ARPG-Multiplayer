using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PredictedTelegraph))]
public class Coldsnap : TelegraphHitEffect {
    private const int BASE_DAMAGE = 16;

    private const float CHILL_DURATION = 2.5f;

    protected override void HandleCharacterHit(NetworkStats caster, NetworkStats hitCharacter) {
        base.HandleCharacterHit(caster, hitCharacter);

        int damage = BASE_DAMAGE;

        BuffController hitBuffController = hitCharacter.GetComponent<BuffController>();

        hitCharacter.TakeDamage(damage, caster.GetComponent<CharacterController>());

        if (hitBuffController.HasBuff("Chill")) {
            hitBuffController.ServerRemoveBuff("Chill");
            hitBuffController.ServerApplyBuff("Frozen");
        } else {
            hitBuffController.ServerApplyBuff("Chill", CHILL_DURATION);
        }
    }
}