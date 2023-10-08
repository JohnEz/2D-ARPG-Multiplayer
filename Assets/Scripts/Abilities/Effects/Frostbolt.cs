using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PredictedProjectile))]
public class Frostbolt : MonoBehaviour {
    private const int BASE_DAMAGE = 10;

    private const int CHILL_DAMAGE = 2;

    private void OnEnable() {
        GetComponent<PredictedProjectile>().OnHit += HandleProjectileHit;
    }

    private void HandleProjectileHit(Vector3 hitLocation, NetworkStats caster, NetworkStats hitCharacter) {
        int damage = BASE_DAMAGE;

        // if target has chill
        // damage += CHILL_DAMAGE;
        // extend chill buff timer by .8f;

        hitCharacter.TakeDamage(damage, caster.IsOwner);
    }
}