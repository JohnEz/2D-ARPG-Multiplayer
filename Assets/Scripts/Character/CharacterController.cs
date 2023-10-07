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

    // maybe a better solution for a client value synced to the server
    [field: SyncVar(ReadPermissions = ReadPermission.ExcludeOwner)]
    public Vector2 InputDirection { get; [ServerRpc(RunLocally = true)] set; }

    //public Vector2 InputDirection {
    //    get { return GetInputDirection(); }
    //    set { SetInputDirection(value); }
    //}

    //private Vector2 _inputDirectionClient;

    //[SyncVar]
    //private Vector2 _inputDirectionServer;

    public Vector2 AimLocation = Vector2.zero;

    private MovementController _movementController;
    private CharacterStateController _stateController;
    private CastController _castController;
    private AbilitiesController _abilitiesController;

    [SerializeField]
    private GameObject visuals;

    //public Vector2 GetInputDirection() {
    //    if (base.IsOwner) {
    //        return _inputDirectionClient;
    //    }

    //    return _inputDirectionServer;
    //}

    //private void SetInputDirection(Vector2 value) {
    //    _inputDirectionClient = value;
    //    SetInputDirectionServer(value);
    //}

    //[ServerRpc]
    //private void SetInputDirectionServer(Vector2 value) {
    //    _inputDirectionServer = value;
    //}

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

    public void UseAbilityOne() {
        CastAbility(0);
    }

    public void UseAbilityTwo() {
        CastAbility(1);
    }

    public void CastAbility(int abilityId) {
        var ability = _abilitiesController.GetAbility(abilityId);
        if (!CanCastAbility(ability)) {
            return;
        }

        _castController.Cast(abilityId);
    }
}