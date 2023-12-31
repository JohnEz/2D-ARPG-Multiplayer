﻿using System.Collections;
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

    [SerializeField]
    private GameObject _landingVFX;

    [SerializeField]
    private AudioClip _landingSFX;

    [SerializeField]
    private AbilityEffect _landAbilityEffect;

    protected LandingSpot _landingSpot;

    public override void OnCastComplete(bool isOwner) {
        base.OnCastComplete(isOwner);

        if (isOwner) {
            Vector3 casterPosition = _caster.transform.position;

            float distanceToAimLocation = Vector3.Distance(casterPosition, _casterController.AimLocation);
            Vector3 aimDirection = ((Vector3)_casterController.AimLocation - casterPosition).normalized;

            float distance = Mathf.Clamp(distanceToAimLocation, MinDistance, MaxDistance);

            _landingSpot = LeapTarget.GetLeapLandingSpot(casterPosition, distance, aimDirection);

            _casterController.StartLeapMovement(_landingSpot.safeSpot, _duration, _leapMoveCurve);
        }

        _casterController.StartLeap(_duration, _leapZCurve, this);
    }

    public virtual void OnLeapComplete() {
        if (_landingVFX) {
            Instantiate(_landingVFX, _casterController.transform.position, Quaternion.identity);
        }

        AudioManager.Instance.PlaySound(_landingSFX, _casterController.transform.position);

        Destroy(gameObject);
    }
}