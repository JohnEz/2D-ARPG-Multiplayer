using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

[RequireComponent(typeof(CharacterController))]
public class PlayerInput : NetworkBehaviour {
    private CharacterController _characterController;

    private Interactor _interactor;

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
        _interactor = GetComponent<Interactor>();
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

        if (InputHandler.Instance.AltAttackTwoPressed) {
            _characterController.CastAbility(3);
        }

        if (InputHandler.Instance.AltAttackThreePressed) {
            _characterController.CastAbility(4);
        }

        if (InputHandler.Instance.UtilityPressed) {
            _characterController.CastUtilityAbility(0);
        }

        if (InputHandler.Instance.UtilityTwoPressed) {
            _characterController.CastUtilityAbility(1);
        }

        TurnToMouse();

        _characterController.InputDirection = InputHandler.Instance.MovementVector.normalized;
        _characterController.AimLocation = new Vector2(InputHandler.Instance.MouseWorldPosition.x, InputHandler.Instance.MouseWorldPosition.y);

        if (InputHandler.Instance.InteractPressed && _interactor.HasInteractionTarget()) {
            _interactor.Interact();

            // TODO this feels like a hacky way to stop the character moving
            _characterController.InputDirection = Vector2.zero;
        }
    }

    private void TurnToMouse() {
        _characterController.FaceDirection(InputHandler.Instance.DirectionToMouse(transform.position));
    }
}