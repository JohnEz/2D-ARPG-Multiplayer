using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PredictedTelegraph))]
public class Coldsnap : MonoBehaviour {
    private const int BASE_DAMAGE = 16;

    private void OnEnable() {
        GetComponent<PredictedTelegraph>().OnHit += HandleCharacterHit;
    }

    private void OnDisable() {
        GetComponent<PredictedTelegraph>().OnHit -= HandleCharacterHit;
    }

    private void HandleCharacterHit(NetworkStats caster, NetworkStats hitCharacter) {
        int damage = BASE_DAMAGE;

        BuffController hitBuffController = hitCharacter.GetComponent<BuffController>();

        if (hitBuffController.HasBuff("Chill")) {
            hitBuffController.ServerRemoveBuff("Chill");
            hitBuffController.ServerApplyBuff("Frozen");
        } else {
            hitBuffController.ServerApplyBuff("Chill");
        }

        hitCharacter.TakeDamage(damage, caster.IsOwner);
    }
}