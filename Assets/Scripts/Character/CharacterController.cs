using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;

[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(CharacterStateController))]
[RequireComponent(typeof(CastController))]
[RequireComponent(typeof(AbilitiesController))]
public class CharacterController : NetworkBehaviour {

    [field: SyncVar(ReadPermissions = ReadPermission.ExcludeOwner)]
    public Vector2 InputDirection { get; [ServerRpc(RunLocally = true)] set; }

    public Vector2 AimLocation = Vector2.zero;

    private MovementController _movementController;
    private CharacterStateController _stateController;
    private CastController _castController;
    private AbilitiesController _abilitiesController;

    [SerializeField]
    private GameObject visuals;

    private void Awake() {
        _movementController = GetComponent<MovementController>();
        _stateController = GetComponent<CharacterStateController>();
        _castController = GetComponent<CastController>();
        _abilitiesController = GetComponent<AbilitiesController>();
    }

    private void Update() {
    }

    private void FixedUpdate() {
        if (!base.IsOwner) {
            return;
        }

        MoveFromInput();

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
}