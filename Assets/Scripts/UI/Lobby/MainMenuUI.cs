using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour {

    [SerializeField]
    private Panel loginPanel;

    [SerializeField]
    private Panel lobbyExplorerPanel;

    [SerializeField]
    private Panel lobbyPanel;

    private void Start() {
        loginPanel.SetPanelEnabled(true, true);
        lobbyExplorerPanel.SetPanelEnabled(false, false);
        lobbyPanel.SetPanelEnabled(false, false);
    }

    private void OnEnable() {
        LobbyManager.Instance.OnLoggedIn += HandleLoggedIn;

        NetworkManagerHooks.Instance.OnClientConnected += HandleJoinedLobby;
        NetworkManagerHooks.Instance.OnClientDisconnected += HandleDisconnectLobby;
    }

    private void OnDisable() {
        if (LobbyManager.Instance) {
            LobbyManager.Instance.OnLoggedIn -= HandleLoggedIn;
        }

        if (NetworkManagerHooks.Instance) {
            NetworkManagerHooks.Instance.OnClientConnected -= HandleJoinedLobby;
            NetworkManagerHooks.Instance.OnClientDisconnected -= HandleDisconnectLobby;
        }
    }

    private void HandleLoggedIn() {
        loginPanel.SetPanelEnabled(false);
        lobbyExplorerPanel.SetPanelEnabled(true, true, Panel.OUT_ANIMATION_DURATION); // TODO this delay should be handled by a window / panel manager
    }

    private void HandleJoinedLobby() {
        lobbyExplorerPanel.SetPanelEnabled(false);
        lobbyPanel.SetPanelEnabled(true, true, Panel.OUT_ANIMATION_DURATION);
    }

    private void HandleDisconnectLobby() {
        lobbyExplorerPanel.SetPanelEnabled(true);
        lobbyPanel.SetPanelEnabled(false, true, Panel.OUT_ANIMATION_DURATION);
    }
}