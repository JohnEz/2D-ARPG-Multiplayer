using System.Collections;
using UnityEngine;

public class PlagueBolt : TelegraphHitEffect {
    private const float POWER_SCALING = 2.5f;

    protected override void HandleCharacterHit(NetworkStats caster, NetworkStats hitCharacter) {
        base.HandleCharacterHit(caster, hitCharacter);

        BuffController hitBuffController = hitCharacter.GetComponent<BuffController>();

        caster.DealDamageTo(gameObject.name, hitCharacter, POWER_SCALING);
    }
}