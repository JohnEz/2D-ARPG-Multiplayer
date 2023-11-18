using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using DG.Tweening;
using System;

[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(CharacterStateController))]
[RequireComponent(typeof(CastController))]
[RequireComponent(typeof(AbilitiesController))]
public class CharacterController : NetworkBehaviour {

    // TODO this complains if there is no client as we are calling a server rpc
    [field: SyncVar(ReadPermissions = ReadPermission.ExcludeOwner)]
    public Vector2 InputDirection { get; [ServerRpc(RequireOwnership = false, RunLocally = true)] set; }

    [field: SyncVar(ReadPermissions = ReadPermission.ExcludeOwner)]
    public Vector2 AimLocation { get; [ServerRpc(RequireOwnership = false, RunLocally = true)] set; }

    public Vector3 AimDirection {
        get {
            return ((Vector3)AimLocation - transform.position).normalized;
        }
    }

    // TODO this probably should come from the persistent player
    [SyncVar(OnChange = nameof(HandleUsernameChange))]
    public string Username;

    public event Action OnUsernameChanged;

    private MovementController _movementController;
    private CharacterStateController _stateController;
    private CastController _castController;
    private AbilitiesController _abilitiesController;
    private BuffController _buffController;
    private NetworkStats _stats;

    [SerializeField]
    private GameObject visuals;

    private Collider2D _hitbox;

    private Rigidbody2D _rigidBody;

    private LeapAbility _leapAbility;

    public Transform FacingTransform {
        get {
            return visuals.transform;
        }
    }

    [SerializeField]
    private AudioClip _deathSFX;

    private float _timeDashing = 0f;
    private Vector2 _dashDirection = Vector2.zero;
    private float _dashDuration = 0f;
    private float _dashSpeed = 0f;
    private AnimationCurve _dashCurveSpeed;

    private void Awake() {
        _movementController = GetComponent<MovementController>();
        _stateController = GetComponent<CharacterStateController>();
        _castController = GetComponent<CastController>();
        _abilitiesController = GetComponent<AbilitiesController>();
        _buffController = GetComponent<BuffController>();
        _stats = GetComponent<NetworkStats>();
        _hitbox = GetComponent<Collider2D>();
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    private void OnEnable() {
        _stats.OnHealthDepleted += HandleHealthDepleted;
        _buffController.OnStunApplied += HandleStunApplied;
        _buffController.OnStunExpired += HandleStunExpired;
    }

    private void OnDisable() {
        _stats.OnHealthDepleted -= HandleHealthDepleted;
        _buffController.OnStunApplied -= HandleStunApplied;
        _buffController.OnStunExpired -= HandleStunExpired;
    }

    public void HandleUsernameChange(string previousName, string newName, bool asServer) {
        OnUsernameChanged?.Invoke();
    }

    private void Update() {
        if (!base.IsOwner && !(base.OwnerId == -1 && IsServer)) {
            return;
        }

        if (_stateController.IsDashing()) {
            DashingUpdate();
        }

        if (_stateController.IsStunned() || _stateController.IsDead()) {
            return;
        }

        if (!_stateController.IsDashing()) {
            MoveFromInput();
        }

        if (!_stateController.IsDashing() && !_stateController.IsCasting() && !_stateController.IsLeaping() && !_stateController.IsChanneling()) {
            _stateController.State = CharacterState.Idle;
        }
    }

    private void FixedUpdate() {
    }

    private void HandleHealthDepleted() {
        _stateController.State = CharacterState.Dead;

        visuals.SetActive(false);
        _rigidBody.isKinematic = true;
        _hitbox.enabled = false;

        _movementController.MoveDirection = Vector2.zero;
        // disable hud

        AudioClipOptions options = new AudioClipOptions();
        options.Volume = 0.5f;

        AudioManager.Instance.PlaySound(_deathSFX, transform, options);
    }

    private void HandleStunApplied() {
        _stateController.SetState(CharacterState.Stunned);
        _movementController.MoveDirection = Vector2.zero;
    }

    private void HandleStunExpired() {
        _stateController.SetState(CharacterState.Idle);
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
        return abilityToCast && _stateController.CanCast() && abilityToCast.CanCast();
    }

    public void CastAbility(int abilityId) {
        var ability = _abilitiesController.GetAbility(abilityId);
        if (!CanCastAbility(ability)) {
            return;
        }

        _castController.Cast(abilityId);
    }

    #region Leaping

    public void StartLeapMovement(Vector2 leapTarget, float leapDuration, AnimationCurve leapMoveCurve) {
        if (!IsOwner) {
            return;
        }
        // TODO this should be the body not the transform
        transform.DOLocalMoveY(leapTarget.y, leapDuration).SetEase(leapMoveCurve);
        transform.DOLocalMoveX(leapTarget.x, leapDuration).SetEase(leapMoveCurve);
    }

    public void StartLeap(float leapDuration, AnimationCurve leapZCurve, LeapAbility leap) {
        _leapAbility = leap;

        _stateController.State = CharacterState.Leaping;

        transform.DOScale(1.75f, leapDuration).SetEase(leapZCurve).OnComplete(LeapComplete);

        _rigidBody.isKinematic = true;
        _hitbox.enabled = false;
    }

    private void LeapComplete() {
        _stateController.State = CharacterState.Idle;
        _rigidBody.isKinematic = false;
        _hitbox.enabled = true;
        _leapAbility.OnLeapComplete();
        _leapAbility = null;
    }

    #endregion Leaping

    #region Dashing

    public void StartDashing(Vector2 direction, AnimationCurve speedCurve, float duration, float speed) {
        _stateController.State = CharacterState.Dashing;
        _movementController.MoveDirection = Vector2.zero;
        _timeDashing = 0f;

        _dashDirection = direction;
        _dashDuration = duration;
        _dashSpeed = speed;
        _dashCurveSpeed = speedCurve;
    }

    private void DashingUpdate() {
        _timeDashing += Time.fixedDeltaTime;
        float currentCurveSpeed = _dashCurveSpeed.Evaluate(_timeDashing / _dashDuration);
        Vector3 newPosition = _rigidBody.position + (_dashDirection * currentCurveSpeed * _dashSpeed * Time.fixedDeltaTime);

        _rigidBody.MovePosition(newPosition);

        if (_timeDashing >= _dashDuration) {
            EndDashing();
        }
    }

    private void EndDashing() {
        if (!_stateController.IsDashing()) {
            Debug.LogError($"EndDashing called when wasnt in dashing state {gameObject.name}");
            return;
        }

        _stateController.State = CharacterState.Idle;
    }

    #endregion Dashing
}