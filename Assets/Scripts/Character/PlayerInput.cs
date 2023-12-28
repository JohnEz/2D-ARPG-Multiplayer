using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

[RequireComponent(typeof(CharacterController))]
public class PlayerInput : NetworkBehaviour {
    private CharacterController _characterController;

    public override void OnOwnershipClient(NetworkConnection prevOwner) {
        base.OnOwnershipClient(prevOwner);

        if (!base.Owner.IsLocalClient) {
            GetComponent<PlayerInput>().enabled = false;
            return;
        }

        CameraManager.Instance.SetFollowTarget(gameObject);
        ActionBarManager.Instance.SetCharacter(GetComponent<AbilitiesController>());
    }

    private void Awake() {
        _characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    private void Update() {
        if (MenuManager.Instance && MenuManager.Instance.IsBlockingMenuOpen()) {
            return;
        }

        if (InputHandler.Instance.MovementUpPressed || InputHandler.Instance.MovementDownPressed || InputHandler.Instance.MovementLeftPressed || InputHandler.Instance.MovementRightPressed) {
            // TODO removed until melee isnt cancelled
            //_characterController.ManualCancelCast();
        }

        if (InputHandler.Instance.AttackPressed) {
            _characterController.CastAbility(0);
        }

        if (InputHandler.Instance.AltAttackPressed) {
            _characterController.CastAbility(1);
        }

        if (InputHandler.Instance.DashPressed) {
            _characterController.CastAbility(2);
        }

        if (InputHandler.Instance.UtilityPressed) {
            _characterController.CastAbility(3);
        }

        if (InputHandler.Instance.UtilityTwoPressed) {
            _characterController.CastAbility(4);
        }

        TurnToMouse();

        _characterController.InputDirection = InputHandler.Instance.MovementVector.normalized;
        _characterController.AimLocation = new Vector2(InputHandler.Instance.MouseWorldPosition.x, InputHandler.Instance.MouseWorldPosition.y);
    }

    private void TurnToMouse() {
        _characterController.FaceDirection(InputHandler.Instance.DirectionToMouse(transform.position));
    }
}