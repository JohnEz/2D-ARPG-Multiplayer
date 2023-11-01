using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CourageousLeapLand : TelegraphHitEffect {
    private const int BASE_DAMAGE = 22;

    private float _baseValiantDuration = 1f;

    private float _valiantDurationPerHit = .75f;

    protected override void HandleHit(NetworkStats caster, List<NetworkStats> hitCharacters) {
        base.HandleHit(caster, hitCharacters);

        BuffController buffController = caster.GetComponent<BuffController>();

        float valiantDuration = _baseValiantDuration;

        if (hitCharacters.Count > 0) {
            valiantDuration += _valiantDurationPerHit * hitCharacters.Count;
        }

        buffController.ApplyBuff(buffController, "Valiant", valiantDuration);
    }

    protected override void HandleCharacterHit(NetworkStats caster, NetworkStats hitCharacter) {
        base.HandleCharacterHit(caster, hitCharacter);

        int damage = BASE_DAMAGE;

        hitCharacter.TakeDamage(damage, caster.GetComponent<CharacterController>());
    }
}