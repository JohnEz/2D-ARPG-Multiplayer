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
    private AbilitiesController _abilitiesController;

    private float _maxRange;
    private float _minRange;
    private float _idealRange;
    private const float _idealRangeBuffer = .5f;

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
        _abilitiesController = GetComponent<AbilitiesController>();
    }

    private void Start() {
        _maxRange = _abilitiesController.AbilityList[_abilitiesController.AbilityList.Count - 1].AiDetails.AbilityRange;
        _minRange = _maxRange - (_maxRange * _idealRangeBuffer);
        _idealRange = _maxRange - (_maxRange - _minRange) / 2;
    }

    public void EnterState() {
    }

    public void ExitState() {
    }

    public void UpdateState() {
        _characterController.TurnToFaceTarget(_brain.TargetCharacter.transform);

        AbilityUpdate();
        MovementUpdate();
    }

    public void AbilityUpdate() {
        if (!_stateController.IsCasting()) {
            int indexToCast = -1;

            // TODO there should be a way that we select the best ability to cast and support allies
            Ability abilityToCast = _abilitiesController.AbilityList.Find(ability => {
                indexToCast++;
                bool inRange = _brain.DistanceToTarget <= ability.AiDetails.AbilityRange;

                return ability.CanCast() && !ability.AiDetails.IsSupportAbility && inRange;
            });

            if (indexToCast != -1 && abilityToCast != null) {
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

    public void MovementUpdate() {
        bool inIdealRange = _brain.DistanceToTarget >= _minRange && _brain.DistanceToTarget <= _maxRange;
        bool hasLineOfSight = _brain.HasLineOfSightToTarget();

        if (inIdealRange && hasLineOfSight) {
            _movement.Stop();
            return;
        }

        bool tooClose = _brain.DistanceToTarget < _minRange;
        bool tooFar = _brain.DistanceToTarget > _idealRange;

        if (tooClose) {
            _movement.MoveAwayFromTarget(_brain.TargetCharacter.transform);
        } else if ((tooFar && !_movement.isChasing) || !hasLineOfSight) {
            _movement.SetChaseTarget(_brain.TargetCharacter.transform);
        }
    }
}