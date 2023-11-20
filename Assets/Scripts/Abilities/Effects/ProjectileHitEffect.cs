using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Projectile))]
public class ProjectileHitEffect : MonoBehaviour {

    private void OnEnable() {
        GetComponent<Projectile>().OnHit += HandleProjectileHit;
        GetComponent<Projectile>().OnHitLocation += HandleProjectileHitLocation;
    }

    private void OnDisable() {
        GetComponent<Projectile>().OnHit -= HandleProjectileHit;
        GetComponent<Projectile>().OnHitLocation -= HandleProjectileHitLocation;
    }

    protected virtual void HandleProjectileHit(Vector3 hitLocation, NetworkStats caster, NetworkStats hitCharacter) {
    }

    protected virtual void HandleProjectileHitLocation(Vector3 hitLocation, NetworkStats caster) {
    }
}