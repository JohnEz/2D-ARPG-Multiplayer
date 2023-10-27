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
    }

    private void OnEnable() {
        LobbyManager.Instance.OnLoggedIn += HandleLoggedIn;

        NetworkManagerHooks.Instance.OnConnected += HandleJoinedLobby;
        NetworkManagerHooks.Instance.OnDisconnected += HandleDisconnectLobby;
    }

    private void OnDisable() {
        if (LobbyManager.Instance) {
            LobbyManager.Instance.OnLoggedIn -= HandleLoggedIn;
        }

        NetworkManagerHooks.Instance.OnConnected -= HandleJoinedLobby;
        NetworkManagerHooks.Instance.OnDisconnected -= HandleDisconnectLobby;
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