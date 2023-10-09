using System.Collections;
using UnityEngine;

public class Arrow : MonoBehaviour {
    private const int BASE_DAMAGE = 8;

    private void OnEnable() {
        GetComponent<PredictedProjectile>().OnHit += HandleProjectileHit;
    }

    private void OnDisable() {
        GetComponent<PredictedProjectile>().OnHit -= HandleProjectileHit;
    }

    private void HandleProjectileHit(Vector3 hitLocation, NetworkStats caster, NetworkStats hitCharacter) {
        int damage = BASE_DAMAGE;

        hitCharacter.TakeDamage(damage, caster.IsOwner);
    }
}