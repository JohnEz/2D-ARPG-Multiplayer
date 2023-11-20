using FishNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleansingPotion : ProjectileHitEffect {
    private const float RADIUS = 2f;

    protected override void HandleProjectileHitLocation(Vector3 hitLocation, NetworkStats caster) {
        List<NetworkStats> hitCharacters = PredictedTelegraph.GetCircleHitTargets(hitLocation, RADIUS, caster, true, true, true);

        hitCharacters.ForEach(hitCharacter => {
            BuffController hitBuffController = hitCharacter.GetComponent<BuffController>();

            bool isEnemy = hitCharacter.Faction != caster.Faction;
            bool shouldDispelPositiveBuffs = isEnemy;
            bool shouldDispelNegativeBuffs = !isEnemy;

            hitBuffController.ServerDispellBuffs(shouldDispelPositiveBuffs, shouldDispelNegativeBuffs, -1);

            if (!isEnemy) {
                return;
            }

            //if (hitCharacter.IsOwner || (hitCharacter.OwnerId == -1 && InstanceFinder.IsServer)) {
            //    Vector2 direction = (hitCharacter.transform.position - hitLocation).normalized;

            //    hitCharacter.GetComponent<Rigidbody2D>().AddForce(direction * 60, ForceMode2D.Impulse);
            //}
        });

        if (caster.IsOwner) {
            CameraManager.Instance.ShakeCamera(1.25f, 0.15f);
        }
    }
}