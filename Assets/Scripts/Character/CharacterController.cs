using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using DG.Tweening;

[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(CharacterStateController))]
[RequireComponent(typeof(CastController))]
[RequireComponent(typeof(AbilitiesController))]
public class CharacterController : NetworkBehaviour {

    // TODO this complains if there is no client as we are calling a server rpc
    [field: SyncVar(ReadPermissions = ReadPermission.ExcludeOwner)]
    public Vector2 InputDirection { get; [ServerRpc(RequireOwnership = false, RunLocally = true)] set; }

    public Vector2 AimLocation = Vector2.zero;

    private MovementController _movementController;
    private CharacterStateController _stateController;
    private CastController _castController;
    private AbilitiesController _abilitiesController;

    [SerializeField]
    private GameObject visuals;

    private Collider2D _hitbox;

    private Rigidbody2D _rigidBody;

    private void Awake() {
        _movementController = GetComponent<MovementController>();
        _stateController = GetComponent<CharacterStateController>();
        _castController = GetComponent<CastController>();
        _abilitiesController = GetComponent<AbilitiesController>();
        _hitbox = GetComponent<Collider2D>();
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update() {
    }

    private void FixedUpdate() {
        if (!base.IsOwner && !(base.OwnerId == -1 && IsServer)) {
            return;
        }

        if (_stateController.IsStunned()) {
            return;
        }

        if (!_stateController.IsDashing()) {
            MoveFromInput();
        }

        if (!_stateController.IsDashing() && !_stateController.IsCasting() && !_stateController.IsLeaping()) {
            _stateController.State = CharacterState.Idle;
        }
    }

    private void MoveFromInput() {
        _movementController.MoveDirection = InputDirection;
    }

    public void TurnToFaceTarget(Transform targetTransform) {
        Vector3 directionToTarget = (targetTransform.transform.position - transform.position).normalized;
        FaceDirection(directionToTarget);
    }

    public void FaceDirection(Vector3 direction) {
        visuals.transform.up = direction;
    }

    public bool CanCastAbility(Ability abilityToCast) {
        return _stateController.CanCast() && abilityToCast.CanCast();
    }

    public void CastAbility(int abilityId) {
        var ability = _abilitiesController.GetAbility(abilityId);
        if (!CanCastAbility(ability)) {
            return;
        }

        _castController.Cast(abilityId);
    }

    public void StartLeapMovement(Vector2 leapTarget, float leapDuration, AnimationCurve leapMoveCurve) {
        if (!IsOwner) {
            return;
        }
        transform.DOLocalMoveY(leapTarget.y, leapDuration).SetEase(leapMoveCurve);
        transform.DOLocalMoveX(leapTarget.x, leapDuration).SetEase(leapMoveCurve);
    }

    public void StartLeap(float leapDuration, AnimationCurve leapZCurve) {
        _stateController.State = CharacterState.Leaping;

        transform.DOScaleZ(1.75f, leapDuration).SetEase(leapZCurve).OnComplete(LeapComplete);

        _rigidBody.isKinematic = true;
        _hitbox.enabled = false;
    }

    // need to make this an rpc
    private void LeapComplete() {
        _stateController.State = CharacterState.Idle;
        _rigidBody.isKinematic = false;
        _hitbox.enabled = true;
        //_castingAbility.OnComplete();
    }
}