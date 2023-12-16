using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class DashAbility : AbilityEffect {

    [SerializeField]
    private AnimationCurve _dashSpeedCurve;

    [SerializeField]
    private float _duration;

    [SerializeField]
    private float _dashSpeed;

    [SerializeField]
    private GameObject _dashVFX;

    [SerializeField]
    private GameObject _hitboxPrefab;

    public override void OnCastComplete(bool isOwner) {
        base.OnCastComplete(isOwner);

        Vector2 direction = _casterController.AimDirection;

        if (isOwner) {
            _casterController.StartDashing(direction, _dashSpeedCurve, _duration, _dashSpeed);
        }

        GameObject shieldSlamVFX = Instantiate(_dashVFX, _caster.transform);
        shieldSlamVFX.transform.up = direction;
        Destroy(shieldSlamVFX, _duration);

        GameObject hitbox = Instantiate(_hitboxPrefab, _caster.transform);
        hitbox.transform.up = direction;
        hitbox.GetComponent<Hitbox>().Initialise(_caster.GetComponent<NetworkStats>(), _duration);
    }
}