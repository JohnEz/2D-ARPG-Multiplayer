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
    }

    private void FixedUpdate() {
        if (MoveDirection.magnitude == 0) {
            return;
        }

        float moveSpeed = _stateController.IsCasting() ? _myStats.Speed.CurrentValue * _castController.castingAbility.SpeedWhileCasting : _myStats.Speed.CurrentValue;

        Vector3 newPosition = _body.position + (MoveDirection * moveSpeed) * Time.fixedDeltaTime;

        _body.MovePosition(newPosition);
    }
}