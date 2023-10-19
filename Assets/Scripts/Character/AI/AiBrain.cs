using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;
using System.Linq;
using System;

public class AiBrain : NetworkBehaviour {

    // TODO these should depend on if the creature is a boss or something
    private float _aggroRange = 12f;

    private float _combatRange = 9f;

    private Vector3 _startPosition;
    private CharacterController _myCharacterController;

    private Dictionary<CharacterController, int> _aggroTable = new Dictionary<CharacterController, int>();

    [HideInInspector]
    public event Action<List<CharacterController>> OnAggroTableChange;

    private CharacterController _target;

    // TODO add on target change
    public CharacterController TargetCharacter {
        get { return _target; }
        set { SetTarget(value); }
    }

    [HideInInspector]
    public event Action OnTargetChange;

    public float LeashRange {
        get { return _aggroRange * 1.5f; }
    }

    public bool HasTarget { get { return TargetCharacter != null; } }

    public float DistanceToTarget = Mathf.Infinity;

    public bool IsTargetOutOfLeashRange { get { return DistanceToTarget >= LeashRange; } }

    public bool IsTargetInCombatRange { get { return DistanceToTarget < _combatRange; } }

    private IEnumerator checksCoroutine;

    public override void OnStartClient() {
        base.OnStartClient();

        if (!InstanceFinder.IsServer) {
            GetComponent<AiBrain>().enabled = false;
        }
    }

    private void Awake() {
        _myCharacterController = GetComponent<CharacterController>();
        _startPosition = transform.position;
    }

    private void OnEnable() {
        OnAggroTableChange += HandleAggroTableChange;
        checksCoroutine = CheckForUpdates(.5f);
        StartCoroutine(checksCoroutine);
    }

    private void OnDisable() {
        OnAggroTableChange -= HandleAggroTableChange;
        StopCoroutine(checksCoroutine);
    }

    private IEnumerator CheckForUpdates(float delay) {
        while (true) {
            yield return new WaitForSeconds(delay);

            CalculateDistanceToTarget();
            CheckForNewEnemiesInRange();
        }
    }

    private void CalculateDistanceToTarget() {
        if (!HasTarget) {
            DistanceToTarget = Mathf.Infinity;
            return;
        }

        // TODO - Performance - could use Vector3.sqrMagnitude instead if we need to optimise
        DistanceToTarget = Vector3.Distance(transform.position, TargetCharacter.transform.position);
    }

    private void CheckForNewEnemiesInRange() {
        List<CharacterController> newEnemiesInRange = FindObjectsOfType<CharacterController>().Where(target => {
            return
                target != _myCharacterController &&
                !_aggroTable.ContainsKey(target) &&
                Vector3.Distance(transform.position, target.transform.position) <= _aggroRange;
        }).ToList();

        if (newEnemiesInRange.Count <= 0) {
            return;
        }

        newEnemiesInRange.ForEach(newEnemy => {
            _aggroTable[newEnemy] = 0;
        });

        OnAggroTableChange?.Invoke(newEnemiesInRange);
    }

    private void HandleAggroTableChange(List<CharacterController> updatedCharacters) {
        if (!HasTarget) {
            TargetCharacter = GetHighestAggro(updatedCharacters);
            return;
        }

        CharacterController newTarget = GetHighestAggro(updatedCharacters);

        if (newTarget != TargetCharacter) {
            // TODO if we pass an empty update list this will be set to null
            // it should be fine as we always pass the target character anyway
            TargetCharacter = newTarget;
        }
    }

    private CharacterController GetHighestAggro(List<CharacterController> charactersToCheck) {
        CharacterController highestAggroCharacter = null;
        int highestAggro = -1;

        if (TargetCharacter) {
            highestAggroCharacter = TargetCharacter;
            highestAggro = _aggroTable[TargetCharacter];
        }

        charactersToCheck.ForEach(target => {
            if (_aggroTable.ContainsKey(target)) {
                int targetAggro = _aggroTable[target];

                if (targetAggro > highestAggro) {
                    highestAggro = targetAggro;
                    highestAggroCharacter = target;
                }
            }
        });

        return highestAggroCharacter;
    }

    public void AddAggro(CharacterController target, int amount) {
        if (_aggroTable.ContainsKey(target)) {
            _aggroTable[target] += amount;
        } else {
            _aggroTable[target] = amount;
        }

        OnAggroTableChange?.Invoke(new List<CharacterController>() { target });
    }

    private void SetTarget(CharacterController target) {
        if (target == null) {
            DistanceToTarget = Mathf.Infinity;
        }

        _target = target;

        Debug.Log($"new target: {target.gameObject.name}");
    }

    // TODO delete
    private void AttackTarget() {
        _myCharacterController.TurnToFaceTarget(TargetCharacter.transform);

        NetworkStats _targetStats = TargetCharacter.GetComponent<NetworkStats>();
        CharacterStateController _targetState = TargetCharacter.GetComponent<CharacterStateController>();
        //CastController _targetCastController = _targetCharacter.GetComponent<CastController>();

        _myCharacterController.AimLocation = GetAimLocation(
            transform.position,
            20f, // get projectile speed from ability
            TargetCharacter.transform.position,
            _targetStats.Speed.CurrentValue,
            TargetCharacter.InputDirection,
            _targetState.IsCasting(),
            0.1f // get this from the ability being cast
        );

        _myCharacterController.CastAbility(0);
    }

    public static Vector2 GetAimLocation(Vector3 myPosition, float projectileSpeed, Vector3 targetsPosition, float targetsMoveSpeed, Vector3 targetsMovementDirection, bool isCasting, float speedWhileCasting) {
        Vector3 targetVelocity = targetsMovementDirection * targetsMoveSpeed;

        if (isCasting) {
            targetVelocity *= speedWhileCasting;
        }

        float dist = (targetsPosition - myPosition).magnitude;

        Vector3 targetLocation = targetsPosition + (dist / projectileSpeed) * targetVelocity;

        Debug.DrawLine(targetsPosition, targetLocation);

        return targetLocation;
    }

    public bool HasLineOfSight() {
        if (!TargetCharacter) {
            return true;
        }

        Vector3 targetDirection = (TargetCharacter.transform.position - transform.position).normalized;
        float distance = Vector2.Distance(TargetCharacter.transform.position, transform.position) - 0.25f;

        RaycastHit2D raycast = Physics2D.CircleCast(transform.position, .5f, targetDirection, distance, 1 << LayerMask.NameToLayer("Obstacles"));

        return raycast.collider == null;
    }
}