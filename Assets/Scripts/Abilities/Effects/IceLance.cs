using FishNet;
using System.Collections;
using UnityEngine;

public class IceLance : MonoBehaviour {
    private const int BASE_DAMAGE = 22;

    private const int CHILL_DAMAGE = 6;

    private void OnEnable() {
        GetComponent<PredictedProjectile>().OnHit += HandleProjectileHit;
    }

    private void OnDisable() {
        GetComponent<PredictedProjectile>().OnHit -= HandleProjectileHit;
    }

    private void HandleProjectileHit(Vector3 hitLocation, NetworkStats caster, NetworkStats hitCharacter) {
        int damage = BASE_DAMAGE;

        BuffController hitBuffController = hitCharacter.GetComponent<BuffController>();

        if (hitBuffController.HasBuff("Chill")) {
            damage += CHILL_DAMAGE;
        } else {
            hitBuffController.ServerApplyBuff("Chill");
        }

        hitCharacter.TakeDamage(damage, caster.IsOwner);
    }
}