using System.Collections;
using UnityEngine;

public class Longsword : MonoBehaviour {
    private const int BASE_DAMAGE = 9;

    private const int VALIANT_HEALING = 4;

    private void OnEnable() {
        GetComponent<PredictedSlash>().OnHit += HandleCharacterHit;
    }

    private void OnDisable() {
        GetComponent<PredictedSlash>().OnHit -= HandleCharacterHit;
    }

    private void HandleCharacterHit(Vector3 Location, NetworkStats caster, NetworkStats hitCharacter) {
        int damage = BASE_DAMAGE;

        BuffController buffController = caster.GetComponent<BuffController>();
        CharacterController characterController = caster.GetComponent<CharacterController>();

        if (buffController.HasBuff("Valiant")) {
            caster.ReceiveHealing(VALIANT_HEALING, characterController);
        }

        hitCharacter.TakeDamage(damage, characterController);
    }
}