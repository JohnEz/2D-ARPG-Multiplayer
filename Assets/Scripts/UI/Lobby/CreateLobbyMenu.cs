using UnityEngine;
using System.Collections;
using TMPro;

public class CreateLobbyMenu : MonoBehaviour {

    [SerializeField]
    private TMP_InputField lobbyNameInput;

    [SerializeField]
    private AsyncButton _createButton;

    private bool pendingLobbyCreate = false;

    public void OnEnable() {
        //TODO fill the input field with the last used username

        lobbyNameInput.interactable = true;
        NetworkManagerHooks.Instance.OnConnected += HandleCreateComplete;
    }

    public void OnDisable() {
        NetworkManagerHooks.Instance.OnConnected -= HandleCreateComplete;
    }

    public async void CreateLobby() {
        if (pendingLobbyCreate) {
            return;
        }

        pendingLobbyCreate = true;

        HandleCreateStart();

        bool lobbyCreated = await LobbyManager.Instance.CreateLobby(lobbyNameInput.text);

        pendingLobbyCreate = false;
    }

    private void HandleCreateStart() {
        lobbyNameInput.interactable = false;
        _createButton.SetInteractable(false);
    }

    private void HandleCreateComplete() {
        // this feels wrong as we dont want to reactive them if its successful
        lobbyNameInput.interactable = false;
        _createButton.SetInteractable(true);
    }
}