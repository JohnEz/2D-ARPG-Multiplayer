using System.Collections;
using UnityEngine;

public class PlagueBolt : TelegraphHitEffect {
    private const int BASE_DAMAGE = 25;
    private const float POWER_SCALING = 1.5f;

    protected override void HandleCharacterHit(NetworkStats caster, NetworkStats hitCharacter) {
        base.HandleCharacterHit(caster, hitCharacter);

        BuffController hitBuffController = hitCharacter.GetComponent<BuffController>();

        caster.DealDamageTo(gameObject.name, hitCharacter, BASE_DAMAGE, POWER_SCALING);
    }
}