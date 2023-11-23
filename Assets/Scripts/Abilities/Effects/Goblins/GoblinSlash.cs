using System.Collections;
using UnityEngine;

public class GoblinSlash : MonoBehaviour {
    private const int BASE_DAMAGE = 10;

    private void OnEnable() {
        GetComponent<PredictedSlash>().OnHit += HandleCharacterHit;
    }

    private void OnDisable() {
        GetComponent<PredictedSlash>().OnHit -= HandleCharacterHit;
    }

    private void HandleCharacterHit(Vector3 Location, NetworkStats caster, NetworkStats hitCharacter) {
        int damage = BASE_DAMAGE;

        CharacterController characterController = caster.GetComponent<CharacterController>();

        hitCharacter.TakeDamage(damage, characterController);
    }
}