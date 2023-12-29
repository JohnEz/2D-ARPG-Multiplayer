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
    private MenuType _menu;

    public bool Interact(Interactor interactor) {
        MenuManager.Instance.OpenMenu(MenuType.MAIN_MENU);
        return true;
    }
}