using System.Collections;
using UnityEngine;

public class InteractorMenu : MonoBehaviour, IInteractable {

    [SerializeField]
    private string _prompt;

    [SerializeField]
    private Sprite _icon;

    public string InteractionPrompt => _prompt;

    public Sprite InteractionIcon => _icon;

    [SerializeField]
    private string _menuId;

    public bool Interact(Interactor interactor) {
        MenuManager.Instance.OpenMenu(_menuId);
        return true;
    }
}