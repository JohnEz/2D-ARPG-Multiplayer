using System.Collections;
using UnityEngine;

public class LeapAbility : AbilityEffect {

    [SerializeField]
    private float _distance;

    [SerializeField]
    private float _duration;

    [SerializeField]
    private AnimationCurve _leapMoveCurve;

    [SerializeField]
    private AnimationCurve _leapZCurve;

    public override void OnCastComplete(bool isOwner) {
        if (isOwner) {
            Vector3 casterPosition = _caster.transform.position;

            LandingSpot landingSpot = LeapTarget.GetLeapLandingSpot(casterPosition, _distance, InputHandler.Instance.DirectionToMouse(casterPosition));

            _caster.StartLeapMovement(landingSpot.safeSpot, _duration, _leapMoveCurve);
        }

        _caster.StartLeap(_duration, _leapZCurve);
    }

    private void OnLeapComplete() {
        Destroy(gameObject);
    }
}