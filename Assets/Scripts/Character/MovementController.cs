using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CharacterStateController))]
public class MovementController : MonoBehaviour {
    private const float CASTING_SPEED_MODIFIER = 0.1f;

    private Rigidbody2D _body;
    private CharacterStateController _stateController;
    private NetworkStats _myStats;

    public Vector2 MoveDirection;

    private void Awake() {
        _body = GetComponent<Rigidbody2D>();
        _myStats = GetComponent<NetworkStats>();
        _stateController = GetComponent<CharacterStateController>();
    }

    private void FixedUpdate() {
        if (MoveDirection.magnitude == 0) {
            return;
        }

        float moveSpeed = _stateController.IsCasting() ? _myStats.MoveSpeed * CASTING_SPEED_MODIFIER : _myStats.MoveSpeed;

        Vector3 newPosition = _body.position + (MoveDirection * moveSpeed) * Time.fixedDeltaTime;

        _body.MovePosition(newPosition);
    }
}