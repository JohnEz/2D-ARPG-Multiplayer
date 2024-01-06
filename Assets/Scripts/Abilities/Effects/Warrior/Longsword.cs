using System.Collections;
using UnityEngine;

public class Longsword : MonoBehaviour {
    private const int BASE_DAMAGE = 7;
    private const float POWER_SCALING = 0.2f;

    private const int VALIANT_HEALING = 3;
    private const float VALIANT_POWER_SCALING = 0.1f;

    private void OnEnable() {
        GetComponent<PredictedSlash>().OnHit += HandleCharacterHit;
    }

    private void OnDisable() {
        GetComponent<PredictedSlash>().OnHit -= HandleCharacterHit;
    }

    private void HandleCharacterHit(Vector3 Location, NetworkStats caster, NetworkStats hitCharacter) {
        BuffController buffController = caster.GetComponent<BuffController>();

        if (buffController.HasBuff("Valiant")) {
            caster.GiveHealingTo(gameObject.name, caster, VALIANT_HEALING, VALIANT_POWER_SCALING);
        }

        caster.DealDamageTo(gameObject.name, hitCharacter, BASE_DAMAGE, POWER_SCALING);
    }
}