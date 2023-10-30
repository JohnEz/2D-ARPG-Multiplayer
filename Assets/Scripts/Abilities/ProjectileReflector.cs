using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ProjectileReflector : MonoBehaviour {
    private CharacterController _caster;

    public void SetCaster(CharacterController caster) {
        _caster = caster;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag != "Projectile") {
            return;
        }

        PredictedProjectile projectile = collision.gameObject.GetComponent<PredictedProjectile>();

        NetworkStats casterStats = _caster.GetComponent<NetworkStats>();

        if (projectile != null && projectile.ShouldReflect(casterStats)) {
            projectile.Reflect(transform.position, _caster);
        }
    }
}