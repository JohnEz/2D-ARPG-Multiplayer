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
    private CharacterStateController _stateController;
    private CastController _castController;

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
        _stateController = GetComponent<CharacterStateController>();
        _castController = GetComponent<CastController>();
    }

    public void EnterState() {
    }

    public void ExitState() {
    }

    public void UpdateState() {
        _characterController.TurnToFaceTarget(_brain.TargetCharacter.transform);

        if (!_stateController.IsCasting()) {
            AbilitiesController abilitiesController = _characterController.GetComponent<AbilitiesController>();

            int indexToCast = -1;

            // TODO there should be a way that we select the best ability to cast and support allies
            abilitiesController.GetAbilities().Find(ability => {
                indexToCast++;
                return ability.CanCast() && !ability.AiDetails.IsSupportAbility;
            });

            if (indexToCast != -1) {
                _characterController.CastAbility(indexToCast);
            }
            return;
        }

        NetworkStats _targetStats = _brain.TargetCharacter.GetComponent<NetworkStats>();
        CharacterStateController _targetState = _brain.TargetCharacter.GetComponent<CharacterStateController>();
        CastController _targetCastController = _brain.TargetCharacter.GetComponent<CastController>();

        _characterController.AimLocation = AiBrain.GetAimLocation(
            transform.position,
            _castController.castingAbility.AiDetails.ProjectileSpeed,
            _brain.TargetCharacter.transform.position,
            _targetStats.Speed.CurrentValue,
            _brain.TargetCharacter.InputDirection,
            _targetState.IsCasting(),
            _targetCastController.castingAbility?.SpeedWhileCasting ?? 1f
        );
    }
}