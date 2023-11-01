using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Hitbox))]
public class ShieldSlamHit : MonoBehaviour {

    [SerializeField]
    private AudioClip _hitSFX;

    private int _damage = 14;

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

        hitCharacter.TakeDamage(_damage, caster.GetComponent<CharacterController>());

        hitCharacter.GetComponent<BuffController>().ServerApplyBuff("Stunned", 1f);

        BuffController casterBuffController = caster.GetComponent<BuffController>();

        if (!_hasGainedShield && casterBuffController.HasBuff("Valiant")) {
            caster.GetComponent<BuffController>().ServerApplyBuff("Shielded");
            _hasGainedShield = true;
        }
    }
}