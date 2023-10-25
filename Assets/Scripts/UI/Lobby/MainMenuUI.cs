using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour {

    [SerializeField]
    private GameObject loginPanel;

    [SerializeField]
    private GameObject lobbyExplorerPanel;

    [SerializeField]
    private GameObject lobbyPanel;

    private void Start() {
        loginPanel.SetActive(true);
        lobbyExplorerPanel.SetActive(false);
        lobbyPanel.SetActive(false);

        LobbyManager.Instance.OnLoggedIn += HandleLoggedIn;
        LobbyManager.Instance.OnLobbyJoined += HandleJoinedLobby;
        LobbyManager.Instance.OnLobbyDisconnected += HandleDisconnectLobby;

        //LobbyManager.Instance.OnGameStarted.AddListener(HandleGameStarted);
    }

    private void OnDestroy() {
        if (!LobbyManager.Instance) {
            return;
        }

        LobbyManager.Instance.OnLoggedIn -= HandleLoggedIn;
        LobbyManager.Instance.OnLobbyJoined -= HandleJoinedLobby;
        LobbyManager.Instance.OnLobbyDisconnected -= HandleDisconnectLobby;
        //LobbyManager.Instance.OnGameStarted -= ;
    }

    private void HandleLoggedIn() {
        loginPanel.SetActive(false);
        lobbyExplorerPanel.SetActive(true);
    }

    private void HandleJoinedLobby() {
        lobbyExplorerPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    private void HandleDisconnectLobby() {
        lobbyExplorerPanel.SetActive(true);
        lobbyPanel.SetActive(false);
    }
}