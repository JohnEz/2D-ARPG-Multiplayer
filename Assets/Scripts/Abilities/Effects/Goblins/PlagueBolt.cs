using System.Collections;
using UnityEngine;

public class PlagueBolt : TelegraphHitEffect {
    private const int BASE_DAMAGE = 20;

    protected override void HandleCharacterHit(NetworkStats caster, NetworkStats hitCharacter)
    {
        base.HandleCharacterHit(caster, hitCharacter);

        int damage = BASE_DAMAGE;

        BuffController hitBuffController = hitCharacter.GetComponent<BuffController>();

        hitCharacter.TakeDamage(damage, caster.GetComponent<CharacterController>());
    }

}