using FishNet;
using FishNet.Object;
using UnityEngine;

public enum AIState {
    Idle,
    Chase,
    Combat,
    Leash,
    Dead,
}

[RequireComponent(typeof(AiBrain))]
public class AiStateMachine : NetworkBehaviour {
    private AiBrain _brain;
    private AiMovementPathfinding _movement;
    private AiCombatState _combat;
    private CharacterController _characterController;
    private CharacterStateController _stateController;

    private AIState currentState = AIState.Idle;

    public override void OnStartClient() {
        base.OnStartClient();

        if (!InstanceFinder.IsServer) {
            GetComponent<AiStateMachine>().enabled = false;
        }
    }

    private void Awake() {
        _brain = GetComponent<AiBrain>();
        _movement = GetComponent<AiMovementPathfinding>();
        _combat = GetComponent<AiCombatState>();
        _characterController = GetComponent<CharacterController>();
        _stateController = GetComponent<CharacterStateController>();
    }

    private void Update() {
        // Check for triggers or conditions to change states
        HandleTransitions();

        // Update the current state
        UpdateState();
    }

    private void HandleTransitions() {
        if (_stateController.IsDead()) {
            ChangeState(AIState.Dead);
        }

        // Example transition conditions
        switch (currentState) {
            case AIState.Idle:
                if (_brain.HasTarget) {
                    ChangeState(AIState.Chase);
                }
                break;

            case AIState.Chase:
                if (_brain.IsTargetInCombatRange && _brain.HasLineOfSightToTarget()) {
                    ChangeState(AIState.Combat);
                } else if (!_brain.HasTarget) {
                    ChangeState(AIState.Idle);
                }
                break;

            case AIState.Combat:
                if (!_brain.IsTargetInCombatRange || !_brain.HasLineOfSightToTarget()) {
                    ChangeState(!_brain.HasTarget ? AIState.Chase : AIState.Idle);
                } else if (!_brain.HasTarget) {
                    ChangeState(AIState.Idle);
                }
                break;
        }
    }

    private void ChangeState(AIState newState) {
        if (newState != currentState) {
            ExitState(currentState);

            EnterState(newState);

            currentState = newState;
        }
    }

    private void EnterState(AIState state) {
        // Perform actions when entering a new state
        switch (state) {
            case AIState.Idle:
                break;

            case AIState.Chase:
                if (_brain.HasTarget) {
                    _movement.SetChaseTarget(_brain.TargetCharacter.transform);
                }
                break;

            case AIState.Combat:
                _combat.EnterState();
                break;
        }
    }

    private void ExitState(AIState state) {
        // Perform actions when exiting a state
        // Implement cleanup or any necessary actions here
        switch (state) {
            case AIState.Idle:
                break;

            case AIState.Chase:
                _movement.Stop();
                break;

            case AIState.Combat:
                _combat.ExitState();
                break;
        }
    }

    private void UpdateState() {
        // Update the current state (e.g., for continuous actions)
        switch (currentState) {
            case AIState.Idle:
                break;

            case AIState.Chase:
                if (_brain.HasTarget) {
                    _characterController.TurnToFaceTarget(_brain.TargetCharacter.transform);
                }
                break;

            case AIState.Combat:
                _combat.UpdateState();
                break;
        }
    }
}