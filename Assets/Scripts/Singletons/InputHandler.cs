using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : Singleton<InputHandler> {
    public Vector2 MovementVector { get; private set; }

    public Vector3 MousePosition { get; private set; }
    public Vector3 MouseWorldPosition { get; private set; }

    public bool DashPressed { get; private set; }

    public bool AttackPressed { get; private set; }
    public bool AltAttackPressed { get; private set; }
    public bool AltAttackTwoPressed { get; private set; }
    public bool AltAttackThreePressed { get; private set; }

    public bool UtilityPressed { get; private set; }
    public bool UtilityTwoPressed { get; private set; }

    public bool UltimatePressed { get; private set; }

    public bool MovementUpPressed { get; private set; }
    public bool MovementDownPressed { get; private set; }
    public bool MovementLeftPressed { get; private set; }
    public bool MovementRightPressed { get; private set; }

    public bool InteractPressed { get; private set; }

    private void Update() {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        MovementVector = new Vector2(horizontalInput, verticalInput);

        MousePosition = Input.mousePosition;
        Vector3 rawMouseWorldPosition = Camera.main.ScreenToWorldPoint(MousePosition);
        MouseWorldPosition = new Vector3(rawMouseWorldPosition.x, rawMouseWorldPosition.y, 0);

        AttackPressed = Input.GetKey(KeyCode.Mouse0);
        AltAttackPressed = Input.GetKey(KeyCode.Mouse1);
        DashPressed = Input.GetKey(KeyCode.Space);
        AltAttackTwoPressed = Input.GetKey(KeyCode.Q);
        AltAttackThreePressed = Input.GetKey(KeyCode.E);

        UtilityPressed = Input.GetKey(KeyCode.Alpha1);
        UtilityTwoPressed = Input.GetKey(KeyCode.Alpha2);

        UltimatePressed = Input.GetKeyDown(KeyCode.R);

        MovementUpPressed = Input.GetKeyDown(KeyCode.W);
        MovementDownPressed = Input.GetKeyDown(KeyCode.S);
        MovementLeftPressed = Input.GetKeyDown(KeyCode.A);
        MovementRightPressed = Input.GetKeyDown(KeyCode.D);

        InteractPressed = Input.GetKeyDown(KeyCode.F);
    }

    public Vector2 DirectionToMouse(Vector3 startPosition) {
        return new Vector2(MouseWorldPosition.x - startPosition.x, MouseWorldPosition.y - startPosition.y).normalized;
    }

    public float DistanceToMouse(Vector3 startPosition) {
        return new Vector2(MouseWorldPosition.x - startPosition.x, MouseWorldPosition.y - startPosition.y).magnitude;
    }
}