using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Projectile))]
public class ProjectileHitEffect : MonoBehaviour {

    [SerializeField]
    private GameObject _characterHitVfx;

    private void OnEnable() {
        GetComponent<Projectile>().OnHit += HandleProjectileHit;
        GetComponent<Projectile>().OnHitLocation += HandleProjectileHitLocation;
    }

    private void OnDisable() {
        GetComponent<Projectile>().OnHit -= HandleProjectileHit;
        GetComponent<Projectile>().OnHitLocation -= HandleProjectileHitLocation;
    }

    protected virtual void HandleProjectileHit(Vector3 hitLocation, NetworkStats caster, NetworkStats hitCharacter) {
        if (_characterHitVfx) {
            GameObject hitVFX = Instantiate(_characterHitVfx, hitCharacter.transform);
            hitVFX.transform.position = hitCharacter.transform.position;
        }
    }

    protected virtual void HandleProjectileHitLocation(Vector3 hitLocation, NetworkStats caster) {
    }
}