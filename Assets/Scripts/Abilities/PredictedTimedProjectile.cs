using UnityEngine;
using DG.Tweening;

public class PredictedTimedProjectile : Projectile {

    [SerializeField]
    private float _timeToHitLocation = 1f;

    [SerializeField]
    private AnimationCurve movementCurve;

    [SerializeField]
    private AnimationCurve zMovementCurve;

    public override void InitialiseTargetLocation(Vector3 targetLocation, float passedTime, NetworkStats caster) {
        base.InitialiseTargetLocation(targetLocation, passedTime, caster);

        float timeToComplete = _timeToHitLocation - passedTime;

        _body.DOMove(targetLocation, timeToComplete - passedTime).SetEase(movementCurve).OnComplete(() => HandleHit(targetLocation, false));
        transform.DOScale(2.5f, timeToComplete).SetEase(zMovementCurve);
    }
}