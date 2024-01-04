using TMPro;
using UnityEngine;

public class LoginMenu : MonoBehaviour {

    [SerializeField]
    private TMP_InputField _usernameInput;

    [SerializeField]
    private AsyncButton _loginButton;

    public void OnEnable() {
        //TODO fill the input field with the last used username

        string previousUsername = PlayerPrefs.GetString("Username", "");

        _usernameInput.text = previousUsername;
        _usernameInput.interactable = true;

        _usernameInput.onSubmit.AddListener(HandleInputSubmit);

        LobbyManager.Instance.OnLoggedIn += HandleLoginComplete;
    }

    public void OnDisable() {
        LobbyManager.Instance.OnLoggedIn -= HandleLoginComplete;

        _usernameInput.onSubmit.RemoveListener(HandleInputSubmit);
    }

    public void Login() {
        HandleLoginStart();
        LobbyManager.Instance.Authenticate(_usernameInput.text);
        PlayerPrefs.SetString("Username", _usernameInput.text);
    }

    private void HandleInputSubmit(string value) {
        Login();
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