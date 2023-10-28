using System.Collections;
using UnityEngine;

public class DashAbility : AbilityEffect {

    [SerializeField]
    private AnimationCurve _dashSpeedCurve;

    [SerializeField]
    private float _duration;

    [SerializeField]
    private float _dashSpeed;

    [SerializeField]
    private GameObject _dashVFX;

    public override void OnCastComplete(bool isOwner) {
        base.OnCastComplete(isOwner);

        if (isOwner) {
            _caster.StartDashing(_caster.AimDirection, _dashSpeedCurve, _duration, _dashSpeed);
        }

        //GameObject shieldSlamVFX = Instantiate(dashAbility.prefabs[0], transform);
        //shieldSlamVFX.transform.up = dashDirection;
        //Destroy(shieldSlamVFX, dashAbility.DashDuration);
    }
}