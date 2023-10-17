using FishNet;
using FishNet.Object;
using UnityEngine;

public enum AIState {
    Idle,
    Chase,
    Combat,
    Leash,
}

[RequireComponent(typeof(AiBrain))]
public class AiStateMachine : NetworkBehaviour {
    private AiBrain _brain;

    private AIState currentState = AIState.Idle;

    public override void OnStartClient() {
        base.OnStartClient();

        if (!InstanceFinder.IsServer) {
            GetComponent<AiStateMachine>().enabled = false;
        }
    }

    private void Awake() {
        _brain = GetComponent<AiBrain>();
    }

    private void Update() {
        // Check for triggers or conditions to change states
        HandleTransitions();

        // Update the current state
        UpdateState();
    }

    private void HandleTransitions() {
        // Example transition conditions
        switch (currentState) {
            case AIState.Idle:
                if (_brain.HasTarget) {
                    ChangeState(AIState.Chase);
                }
                break;

            case AIState.Chase:
                if (_brain.IsTargetInCombatRange) {
                    ChangeState(AIState.Combat);
                } else if (!_brain.HasTarget) {
                    ChangeState(AIState.Idle);
                }
                break;

            case AIState.Combat:
                if (!_brain.IsTargetInCombatRange) {
                    ChangeState(AIState.Chase);
                } else if (!_brain.HasTarget) {
                    ChangeState(AIState.Idle);
                }
                break;
        }
    }

    private void ChangeState(AIState newState) {
        Debug.Log($"{gameObject.name} changing to state {newState}");

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
                break;

            case AIState.Combat:
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
                break;

            case AIState.Combat:
                break;
        }
    }

    private void UpdateState() {
        // Update the current state (e.g., for continuous actions)
        switch (currentState) {
            case AIState.Idle:
                break;

            case AIState.Chase:
                // Move towards player
                break;

            case AIState.Combat:
                // run combat script
                break;
        }
    }
}