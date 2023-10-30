using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PredictedProjectile))]
public class ProjectileHitEffect : MonoBehaviour {

    private void OnEnable() {
        GetComponent<PredictedProjectile>().OnHit += HandleProjectileHit;
        GetComponent<PredictedProjectile>().OnHitLocation += HandleProjectileHitLocation;
    }

    private void OnDisable() {
        GetComponent<PredictedProjectile>().OnHit -= HandleProjectileHit;
        GetComponent<PredictedProjectile>().OnHitLocation -= HandleProjectileHitLocation;
    }

    protected virtual void HandleProjectileHit(Vector3 hitLocation, NetworkStats caster, NetworkStats hitCharacter) {
    }

    protected virtual void HandleProjectileHitLocation(Vector3 hitLocation, NetworkStats caster) {
    }
}