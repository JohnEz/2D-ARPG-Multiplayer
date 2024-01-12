using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CourageousLeapLand : TelegraphHitEffect {
    private const float POWER_SCALING = 1.8f;

    private float _baseValiantDuration = 1f;

    private float _valiantDurationPerHit = .75f;

    private string BUFF = "Valiant";

    protected override void HandleHit(NetworkStats caster, List<NetworkStats> hitCharacters) {
        base.HandleHit(caster, hitCharacters);

        BuffController casterBuffController = caster.GetComponent<BuffController>();

        float valiantDuration = _baseValiantDuration;

        if (hitCharacters.Count > 0) {
            valiantDuration += _valiantDurationPerHit * hitCharacters.Count;
        }

        if (casterBuffController.HasBuff(BUFF)) {
            casterBuffController.ServerUpdateBuffDuration(BUFF, valiantDuration);
        } else {
            casterBuffController.ApplyBuffTo(casterBuffController, BUFF, valiantDuration);
        }
    }

    protected override void HandleCharacterHit(NetworkStats caster, NetworkStats hitCharacter) {
        base.HandleCharacterHit(caster, hitCharacter);

        caster.DealDamageTo(gameObject.name, hitCharacter, POWER_SCALING);
    }
}