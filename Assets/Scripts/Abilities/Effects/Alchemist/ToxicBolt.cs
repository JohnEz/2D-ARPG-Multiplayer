using System.Collections;
using UnityEngine;

public class ToxicBolt : ProjectileHitEffect {
    private const int BASE_DAMAGE = 12;

    private const string BUFF = "Toxic";

    protected override void HandleProjectileHit(Vector3 hitLocation, NetworkStats caster, NetworkStats hitCharacter) {
        int damage = BASE_DAMAGE;

        BuffController hitBuffController = hitCharacter.GetComponent<BuffController>();

        hitBuffController.ServerApplyBuff(BUFF);

        hitCharacter.TakeDamage(damage, caster.IsOwner, caster.GetComponent<CharacterController>());
    }
}