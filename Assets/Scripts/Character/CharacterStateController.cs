using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using static UnityEngine.Rendering.DebugUI;

public enum CharacterState {
    Idle,
    Moving,
    Dashing,
    Casting,
    Stunned,
    Dead,
    Leaping,
}

public class CharacterStateController : MonoBehaviour {
    private CharacterState _state;

    public event Action OnDeath;

    public CharacterState State {
        get { return _state; }
        set { SetState(value); }
    }

    public void SetState(CharacterState newState) {
        if (_state == newState) {
            return;
        }

        DebugState(_state, newState);

        _state = newState;

        if (newState == CharacterState.Dead) {
            OnDeath?.Invoke();
        }
    }

    public void Start() {
        State = CharacterState.Idle;
    }

    public bool IsIdle() {
        return State == CharacterState.Idle;
    }

    public bool IsMoving() {
        return State == CharacterState.Moving;
    }

    public bool IsDashing() {
        return State == CharacterState.Dashing;
    }

    public bool IsCasting() {
        return State == CharacterState.Casting;
    }

    public bool IsStunned() {
        return State == CharacterState.Stunned;
    }

    public bool IsDead() {
        return State == CharacterState.Dead;
    }

    public bool IsLeaping() {
        return State == CharacterState.Leaping;
    }

    private void DebugState(CharacterState previousState, CharacterState newState) {
        //print($"{gameObject.name}: {previousState} -> {newState}");
    }

    public bool CanCast() {
        return IsIdle() || IsMoving();
    }
}