using System.Collections;
using UnityEngine;

public class LeapAbility : AbilityEffect {

    [SerializeField]
    public float MinDistance = 0;

    [SerializeField]
    public float MaxDistance;

    [SerializeField]
    private float _duration;

    [SerializeField]
    private AnimationCurve _leapMoveCurve;

    [SerializeField]
    private AnimationCurve _leapZCurve;

    public override void OnCastComplete(bool isOwner) {
        base.OnCastComplete(isOwner);

        if (isOwner) {
            Vector3 casterPosition = _caster.transform.position;

            float distanceToAimLocation = Vector3.Distance(casterPosition, _caster.AimLocation);
            Vector3 aimDirection = ((Vector3)_caster.AimLocation - casterPosition).normalized;

            float distance = Mathf.Clamp(distanceToAimLocation, MinDistance, MaxDistance);

            LandingSpot landingSpot = LeapTarget.GetLeapLandingSpot(casterPosition, distance, aimDirection);

            _caster.StartLeapMovement(landingSpot.safeSpot, _duration, _leapMoveCurve);
        }

        _caster.StartLeap(_duration, _leapZCurve);
    }

    private void OnLeapComplete() {
        Destroy(gameObject);
    }
}