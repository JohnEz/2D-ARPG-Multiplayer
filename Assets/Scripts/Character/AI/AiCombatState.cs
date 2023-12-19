using FishNet;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AICombatState {
    MOVING_TO_LOS,
    ATTACKING
}

public struct AbilityTarget {
    public int abilityIndex;
    public CharacterController target;
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

    private AbilityTarget _abilityTarget;

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

    private AbilityTarget GetAbilityToCast() {
        int abilityIndex = -1;
        AbilityTarget abilityTarget = new AbilityTarget();
        abilityTarget.abilityIndex = -1;

        // TODO there should be a way that we select the best ability to cast and support allies
        Ability abilityToCast = _abilitiesController.AbilityList.Find(ability => {
            abilityIndex++;

            if (!ability.CanCast()) {
                return false;
            }

            // should always be cast if its not on cooldown (mainly for spells that dont require targets, eg totem aoe heal)
            if (ability.AiDetails.IsAutoCast) {
                abilityTarget.abilityIndex = abilityIndex;
                return true;
            }

            CharacterController target = GetAbilityTarget(ability);

            bool hasValidTarget = target != null;

            if (!hasValidTarget) {
                return false;
            }

            bool inRange = Vector2.Distance(target.transform.position, transform.position) <= ability.AiDetails.AbilityRange;

            if (!inRange) {
                return false;
            }

            abilityTarget.abilityIndex = abilityIndex;
            abilityTarget.target = target;

            return true;
        });

        return abilityTarget;
    }

    private CharacterController GetAbilityTarget(Ability ability) {
        if (!ability.AiDetails.IsSupportAbility) {
            return _brain.TargetCharacter;
        }

        return FindAllyForAbility(ability);
    }

    private CharacterController FindAllyForAbility(Ability ability) {
        // can cast on self?
        return _brain.AlliesInRange.Find((ally) => {
            return IsValidTarget(ally, ability.AiDetails.targetConditions);
        });
    }

    private bool IsValidTarget(CharacterController target, AbilityTargetCondition conditions) {
        // TODO allow multiple conditions
        float value = GetCharacterAttributeValue(target, conditions.attribute);

        switch (conditions.AttributeQuantifier) {
            case Quantifier.LT:
            return value < conditions.amount;

            case Quantifier.GT:
            return value > conditions.amount;

            case Quantifier.EQ:
            return value == conditions.amount;

            case Quantifier.LTEQ:
            return value <= conditions.amount;

            case Quantifier.GTEQ:
            return value >= conditions.amount;

            default:
            return false;
        }
    }

    private float GetCharacterAttributeValue(CharacterController target, TargetAttribute attribute) {
        switch (attribute) {
            case TargetAttribute.DISTANCE:
            return Vector2.Distance(target.transform.position, transform.position);

            case TargetAttribute.CURRENT_HEALTH_PERCENT:
            NetworkStats targetStats = target.GetComponent<NetworkStats>();
            return targetStats.CurrentHealth / targetStats.MaxHealth.CurrentValue;

            default:
            return 0f;
        }
    }

    public void AbilityUpdate() {
        if (!_stateController.IsCasting()) {
            AbilityTarget abilityToCast = GetAbilityToCast();

            if (abilityToCast.abilityIndex != -1) {
                _characterController.CastAbility(abilityToCast.abilityIndex);

                _abilityTarget = abilityToCast;
            }
        }

        if (!_abilityTarget.target) {
            return;
        }

        NetworkStats _targetStats = _abilityTarget.target.GetComponent<NetworkStats>();
        CharacterStateController _targetState = _abilityTarget.target.GetComponent<CharacterStateController>();
        CastController _targetCastController = _abilityTarget.target.GetComponent<CastController>();

        _characterController.AimLocation = AiBrain.GetAimLocation(
            transform.position,
            _castController.castingAbility.AiDetails.ProjectileSpeed,
            _abilityTarget.target.transform.position,
            _targetStats.Speed.CurrentValue,
            _abilityTarget.target.InputDirection,
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