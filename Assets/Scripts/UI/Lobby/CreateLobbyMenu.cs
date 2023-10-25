using UnityEngine;
using System.Collections;
using TMPro;

public class CreateLobbyMenu : MonoBehaviour {

    [SerializeField]
    private TMP_InputField lobbyNameInput;

    private bool pendingLobbyCreate = false;

    public async void CreateLobby() {
        if (pendingLobbyCreate) {
            return;
        }

        pendingLobbyCreate = true;

        bool lobbyCreated = await LobbyManager.Instance.CreateLobby(lobbyNameInput.text);

        pendingLobbyCreate = false;
    }
}