using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PredictedProjectile))]
public class ProjectileHitEffect : MonoBehaviour
{

    private void OnEnable() {
        GetComponent<PredictedProjectile>().OnHit += HandleProjectileHit;
    }

    private void OnDisable() {
        GetComponent<PredictedProjectile>().OnHit -= HandleProjectileHit;
    }

    protected virtual void HandleProjectileHit(Vector3 hitLocation, NetworkStats caster, NetworkStats hitCharacter) {
        
    }
}
