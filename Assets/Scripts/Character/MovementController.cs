using FishNet;
using FishNet.Managing.Timing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CharacterStateController))]
public class MovementController : MonoBehaviour {
    private Rigidbody2D _body;
    private CharacterStateController _stateController;
    private NetworkStats _myStats;
    private CastController _castController;

    public Vector2 MoveDirection;

    private void Awake() {
        _body = GetComponent<Rigidbody2D>();
        _myStats = GetComponent<NetworkStats>();
        _stateController = GetComponent<CharacterStateController>();
        _castController = GetComponent<CastController>();

        InstanceFinder.TimeManager.OnTick += Move;
    }

    private void FixedUpdate() {
    }

    private void Move() {
        if (MoveDirection.magnitude == 0) {
            return;
        }

        //Vector3 newPosition = _body.position + (MoveDirection * GetMoveSpeed()) * Time.fixedDeltaTime;

        //_body.MovePosition(newPosition);

        //_body.AddForce(MoveDirection * GetMoveSpeed(), ForceMode2D.Impulse);
        _body.AddForce(MoveDirection * GetMoveSpeed(), ForceMode2D.Impulse);
    }

    private float GetMoveSpeed() {
        if (_stateController.IsDead()) {
            return 0f;
        }

        float speedMod = 1f;

        if (_stateController.IsCasting()) {
            speedMod = _castController.castingAbility.SpeedWhileCasting;
        }

        if (_stateController.IsChanneling()) {
            ChannelAbility channelAbility = _castController.castingAbilityEffect as ChannelAbility;
            speedMod = channelAbility.SpeedWhileChannelingMultiplier;
        }

        return _myStats.Speed.CurrentValue * speedMod;
    }
}