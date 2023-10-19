using FishNet;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AICombatState {
    MOVING_TO_LOS,
    ATTACKING
}

public class AiCombatState : NetworkBehaviour {
    private AiBrain _brain;
    private AiMovementPathfinding _movement;
    private CharacterController _characterController;

    public override void OnStartClient() {
        base.OnStartClient();

        if (!InstanceFinder.IsServer) {
            GetComponent<AiStateMachine>().enabled = false;
        }
    }

    private void Awake() {
        _brain = GetComponent<AiBrain>();
        _movement = GetComponent<AiMovementPathfinding>();
        _characterController = GetComponent<CharacterController>();
    }

    public void EnterState() {
    }

    public void ExitState() {
    }

    public void UpdateState() {
        _characterController.TurnToFaceTarget(_brain.TargetCharacter.transform);

        NetworkStats _targetStats = _brain.TargetCharacter.GetComponent<NetworkStats>();
        CharacterStateController _targetState = _brain.TargetCharacter.GetComponent<CharacterStateController>();
        //CastController _targetCastController = _targetCharacter.GetComponent<CastController>();

        _characterController.AimLocation = AiBrain.GetAimLocation(
            transform.position,
            20f, // get projectile speed from ability
            _brain.TargetCharacter.transform.position,
            _targetStats.Speed.CurrentValue,
            _brain.TargetCharacter.InputDirection,
            _targetState.IsCasting(),
            0.1f // get this from the ability being cast
        );

        _characterController.CastAbility(0);
    }
}