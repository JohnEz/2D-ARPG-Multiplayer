using System.Collections;
using UnityEngine;

public class GoblinSlash : MonoBehaviour {
    private const int BASE_DAMAGE = 5;
    private const float POWER_SCALING = .5f;

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