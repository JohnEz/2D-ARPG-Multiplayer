using System.Collections;
using UnityEngine;

public class Longsword : MonoBehaviour {
    private const float POWER_SCALING = 0.9f;

    private const float VALIANT_POWER_SCALING = 0.4f;

    private void OnEnable() {
        GetComponent<PredictedSlash>().OnHit += HandleCharacterHit;
    }

    private void OnDisable() {
        GetComponent<PredictedSlash>().OnHit -= HandleCharacterHit;
    }

    private void HandleCharacterHit(Vector3 Location, NetworkStats caster, NetworkStats hitCharacter) {
        BuffController buffController = caster.GetComponent<BuffController>();

        if (buffController.HasBuff("Valiant")) {
            caster.GiveHealingTo(gameObject.name, caster, VALIANT_POWER_SCALING);
        }

        caster.DealDamageTo(gameObject.name, hitCharacter, POWER_SCALING);
    }
}