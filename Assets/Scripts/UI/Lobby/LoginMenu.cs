using TMPro;
using UnityEngine;

public class LoginMenu : MonoBehaviour {

    [SerializeField]
    private TMP_InputField _usernameInput;

    [SerializeField]
    private AsyncButton _loginButton;

    public void OnEnable() {
        //TODO fill the input field with the last used username

        _usernameInput.interactable = true;
        LobbyManager.Instance.OnLoggedIn += HandleLoginComplete;
    }

    public void OnDisable() {
        LobbyManager.Instance.OnLoggedIn -= HandleLoginComplete;
    }

    public void Login() {
        HandleLoginStart();
        LobbyManager.Instance.Authenticate(_usernameInput.text);
    }

    private void HandleLoginStart() {
        _usernameInput.interactable = false;
        _loginButton.HandleLoading();
    }

    private void HandleLoginComplete() {
        // this feels wrong as we dont want to reactive them if its successful
        _usernameInput.interactable = false;
        _loginButton.HandleLoadingComplete();
    }
}