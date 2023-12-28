using System.Collections;
using UnityEngine;

public class InteractorMenu : MonoBehaviour, IInteractable {

    [SerializeField]
    private string _prompt;

    [SerializeField]
    private Sprite _icon;

    public string InteractionPrompt => _prompt;

    public Sprite InteractionIcon => _icon;

    public bool Interact(Interactor interactor) {
        Debug.Log($"Interacted with {_prompt}");
        return true;
    }
}