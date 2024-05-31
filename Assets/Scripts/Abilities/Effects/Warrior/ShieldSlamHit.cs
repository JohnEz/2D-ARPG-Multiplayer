using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Hitbox))]
public class ShieldSlamHit : MonoBehaviour {

    [SerializeField]
    private AudioClip _hitSFX;

    private const float POWER_SCALING = 1.6f;

    private const float IMMUNE_SCALING = POWER_SCALING * 1.5f;

    private bool _hasGainedShield = false;

    private void OnEnable() {
        GetComponent<Hitbox>().OnHit += HandleHit;
    }

    private void OnDisable() {
        GetComponent<Hitbox>().OnHit -= HandleHit;
    }

    protected void HandleHit(Vector3 hitLocation, NetworkStats caster, NetworkStats hitCharacter) {
        if (hitCharacter == null) {
            return;
        }

        AudioManager.Instance.PlaySound(_hitSFX, hitLocation);

        bool isHitStunImmune = hitCharacter.IsStunImmune;
        caster.DealDamageTo(gameObject.name, hitCharacter, isHitStunImmune ? IMMUNE_SCALING : POWER_SCALING);

        BuffController hitBuffController = hitCharacter.GetComponent<BuffController>();
        BuffController casterBuffController = caster.GetComponent<BuffController>();

        casterBuffController.ApplyBuffTo(hitBuffController, "Stunned", 1f);

        if (!_hasGainedShield && casterBuffController.HasBuff("Valiant")) {
            casterBuffController.ApplyBuffTo(casterBuffController, "Shielded");
            _hasGainedShield = true;
        }
    }
}