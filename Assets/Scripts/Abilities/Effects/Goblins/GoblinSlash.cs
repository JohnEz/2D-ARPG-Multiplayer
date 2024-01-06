using System.Collections;
using UnityEngine;

public class GoblinSlash : MonoBehaviour {
    private const int BASE_DAMAGE = 8;
    private const float POWER_SCALING = .2f;

    private void OnEnable() {
        GetComponent<PredictedSlash>().OnHit += HandleCharacterHit;
    }

    private void OnDisable() {
        GetComponent<PredictedSlash>().OnHit -= HandleCharacterHit;
    }

    private void HandleCharacterHit(Vector3 Location, NetworkStats caster, NetworkStats hitCharacter) {
        caster.DealDamageTo(gameObject.name, hitCharacter, BASE_DAMAGE, POWER_SCALING);
    }
}