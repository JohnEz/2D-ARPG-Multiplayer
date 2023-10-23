using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;
using System.Linq;
using System;
using UnityEngine.TextCore.Text;

public class AiBrain : NetworkBehaviour {

    // TODO these should depend on if the creature is a boss or something
    private float _aggroRange = 12f;

    private float _combatRange = 11f;

    private Vector3 _startPosition;
    private CharacterController _myCharacterController;
    private CharacterStateController _stateController;
    private NetworkStats _myStats;

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
        _stateController = GetComponent<CharacterStateController>();
        _myStats = GetComponent<NetworkStats>();
        _startPosition = transform.position;
    }

    private void OnEnable() {
        _stateController.OnDeath += HandleDeath;
        OnAggroTableChange += HandleAggroTableChange;
        checksCoroutine = CheckForUpdates(.5f);
        StartCoroutine(checksCoroutine);
    }

    private void OnDisable() {
        _stateController.OnDeath -= HandleDeath;
        OnAggroTableChange -= HandleAggroTableChange;
        StopCoroutine(checksCoroutine);
    }

    private void HandleDeath() {
        OnAggroTableChange -= HandleAggroTableChange;
        _aggroTable.Clear();
        TargetCharacter = null;
        StopCoroutine(checksCoroutine);
    }

    private IEnumerator CheckForUpdates(float delay) {
        while (true) {
            yield return new WaitForSeconds(delay);
            // TODO - Performance - move this out to a manager
            CharacterController[] allCharacters = FindObjectsOfType<CharacterController>();

            CheckForNewEnemiesInRange(allCharacters);
            CauseAlliesToAggro(allCharacters);
        }
    }

    private void Update() {
        CalculateDistanceToTarget();
    }

    private void CalculateDistanceToTarget() {
        if (!HasTarget) {
            DistanceToTarget = Mathf.Infinity;
            return;
        }

        // TODO - Performance - could use Vector3.sqrMagnitude instead if we need to optimise
        DistanceToTarget = Vector3.Distance(transform.position, TargetCharacter.transform.position);
    }

    private void CheckForNewEnemiesInRange(CharacterController[] allCharacters) {
        NetworkStats myStats = GetComponent<NetworkStats>();

        List<CharacterController> newEnemiesInRange = allCharacters.Where(target => {
            NetworkStats targetStats = target.GetComponent<NetworkStats>();
            CharacterStateController targetStateController = target.GetComponent<CharacterStateController>();

            return
                target != _myCharacterController &&
                !targetStateController.IsDead() &&
                targetStats.Faction != myStats.Faction &&
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

    private void CauseAlliesToAggro(CharacterController[] allCharacters) {
        if (!HasTarget) {
            return;
        }

        List<CharacterController> alliesToAggro = allCharacters.Where(character => {
            NetworkStats networkStats = character.GetComponent<NetworkStats>();
            AiBrain aiBrain = character.GetComponent<AiBrain>();

            if (!aiBrain) {
                return false;
            }

            bool isAlly = networkStats.Faction == _myStats.Faction;
            bool isClose = Vector3.Distance(transform.position, character.transform.position) <= _aggroRange;
            bool isInLineOfSight = HasLineOfSightTo(transform, character.transform);
            bool isAlreadyAggroed = aiBrain.HasTarget;

            return isAlly && isClose && isInLineOfSight && !isAlreadyAggroed;
        }).ToList();

        alliesToAggro.ForEach(ally => {
            AiBrain aiBrain = ally.GetComponent<AiBrain>();

            aiBrain.AddAggro(TargetCharacter, 1);
        });
    }

    private void HandleAggroTableChange(List<CharacterController> updatedCharacters) {
        bool previouslyInCombat = _aggroTable.Count > 0;

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

        bool isInCombat = _aggroTable.Count > 0;

        if (!previouslyInCombat && isInCombat) {
            HandlePull();
        }
    }

    private void HandlePull() {
        CharacterController[] allCharacters = FindObjectsOfType<CharacterController>();
        CauseAlliesToAggro(allCharacters);
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
            if (_target != null) {
                _target.GetComponent<CharacterStateController>().OnDeath -= HandleTargetDeath;
            }
        }

        _target = target;

        if (_target) {
            _target.GetComponent<CharacterStateController>().OnDeath += HandleTargetDeath;
            Debug.Log($"new target: {target?.gameObject.name}");
        }
    }

    private void HandleTargetDeath() {
        if (_target == null) {
            return;
        }

        _aggroTable.Remove(_target);
        // TODO should i call that the aggro table has changed instead?
        _target = null;
        TargetCharacter = GetHighestAggro(_aggroTable.Keys.ToList());
    }

    public static Vector2 GetAimLocation(Vector3 myPosition, float projectileSpeed, Vector3 targetsPosition, float targetsMoveSpeed, Vector3 targetsMovementDirection, bool isCasting, float speedWhileCasting) {
        if (projectileSpeed <= 0) {
            return targetsPosition;
        }

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

        return HasLineOfSightTo(transform, TargetCharacter.transform);
    }

    public static bool HasLineOfSightTo(Transform source, Transform target) {
        Vector3 targetDirection = (target.position - source.position).normalized;
        float distance = Vector2.Distance(target.position, source.position) - 0.25f; // Todo - why did i remove half circle radius?

        RaycastHit2D raycast = Physics2D.CircleCast(source.position, .5f, targetDirection, distance, 1 << LayerMask.NameToLayer("Obstacles"));

        return raycast.collider == null;
    }
}