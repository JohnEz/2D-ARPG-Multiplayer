using System.Collections;
using UnityEngine;

public class Longsword : MonoBehaviour {
    private const int BASE_DAMAGE = 9;

    private void OnEnable() {
        GetComponent<PredictedSlash>().OnHit += HandleCharacterHit;
    }

    private void OnDisable() {
        GetComponent<PredictedSlash>().OnHit -= HandleCharacterHit;
    }

    private void HandleCharacterHit(Vector3 Location, NetworkStats caster, NetworkStats hitCharacter) {
        int damage = BASE_DAMAGE;

        Debug.Log($"Hit {hitCharacter.name} for {damage} damage");

        hitCharacter.TakeDamage(damage, caster.IsOwner);
    }
}