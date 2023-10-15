using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;
using System.Linq;

public class AiBrain : NetworkBehaviour {
    private CharacterController _targetCharacter;

    private CharacterController _myCharacterController;

    public override void OnStartClient() {
        base.OnStartClient();

        if (!InstanceFinder.IsServer) {
            GetComponent<AiBrain>().enabled = false;
        }
    }

    private void Awake() {
        _myCharacterController = GetComponent<CharacterController>();
    }

    private void Update() {
        if (_targetCharacter == null) {
            FindTarget();
        }

        if (_targetCharacter != null) {
            AttackTarget();
        }
    }

    private void FindTarget() {
        PlayerInput foundPlayer = FindObjectOfType<PlayerInput>();

        if (foundPlayer) {
            _targetCharacter = foundPlayer.GetComponent<CharacterController>();
        }
    }

    private void FindTargetLEGACY() {
        CharacterController foundPlayer = FindObjectsOfType<CharacterController>()
            .Where((character) => character != _myCharacterController)
            .First();

        if (foundPlayer) {
            _targetCharacter = foundPlayer;
        }
    }

    private void AttackTarget() {
        _myCharacterController.TurnToFaceTarget(_targetCharacter.transform);

        NetworkStats _targetStats = _targetCharacter.GetComponent<NetworkStats>();
        CharacterStateController _targetState = _targetCharacter.GetComponent<CharacterStateController>();
        //CastController _targetCastController = _targetCharacter.GetComponent<CastController>();

        _myCharacterController.AimLocation = GetAimLocation(
            transform.position,
            20f, // get projectile speed from ability
            _targetCharacter.transform.position,
            _targetStats.Speed.CurrentValue,
            _targetCharacter.InputDirection,
            _targetState.IsCasting(),
            0.1f // get this from the ability being cast
        );

        _myCharacterController.CastAbility(0);
    }

    private static Vector2 GetAimLocation(Vector3 myPosition, float projectileSpeed, Vector3 targetsPosition, float targetsMoveSpeed, Vector3 targetsMovementDirection, bool isCasting, float speedWhileCasting) {
        Vector3 targetVelocity = targetsMovementDirection * targetsMoveSpeed;

        if (isCasting) {
            targetVelocity *= speedWhileCasting;
        }

        float dist = (targetsPosition - myPosition).magnitude;

        Vector3 targetLocation = targetsPosition + (dist / projectileSpeed) * targetVelocity;

        Debug.DrawLine(targetsPosition, targetLocation);

        return targetLocation;
    }
}