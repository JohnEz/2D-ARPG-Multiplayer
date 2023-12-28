using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using UnityEngine;

public class Interactor : NetworkBehaviour {

    [SerializeField]
    private Transform _interactionPoint;

    [SerializeField]
    private float _interactionPointRadius;

    [SerializeField]
    private LayerMask _interactableMask;

    [SerializeField]
    private CharacterCanvasController _characterCanvasController;

    private readonly Collider2D[] _colliders = new Collider2D[3];

    private IInteractable _interactable;

    public override void OnOwnershipClient(NetworkConnection prevOwner) {
        base.OnOwnershipClient(prevOwner);

        if (!base.Owner.IsLocalClient) {
            GetComponent<Interactor>().enabled = false;
            return;
        }
    }

    private void Update() {
        int numberOfInteractable = Physics2D.OverlapCircleNonAlloc(_interactionPoint.position, _interactionPointRadius, _colliders, _interactableMask);

        IInteractable newInteractable = numberOfInteractable > 0 ?
            _colliders[0].GetComponent<IInteractable>() :
            null;

        if (newInteractable != _interactable) {
            _interactable = newInteractable;

            if (_interactable != null) {
                HandleNewInteractable();
            } else {
                HandleRemovedInteractable();
            }
        }
    }

    public bool HasInteractionTarget() {
        return _interactable != null;
    }

    public void Interact() {
        if (_interactable == null) {
            return;
        }

        _interactable.Interact(this);
    }

    private void HandleNewInteractable() {
        if (_characterCanvasController == null) {
            return;
        }

        _characterCanvasController.ShowInteractablePrompt(_interactable);
    }

    private void HandleRemovedInteractable() {
        if (_characterCanvasController == null) {
            return;
        }
        _characterCanvasController.HideInteractablePrompt();
    }
}