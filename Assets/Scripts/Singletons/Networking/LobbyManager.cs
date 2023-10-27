using FishNet;
using FishNet.Transporting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

//public class LobbyManager<Player, Lobby> : Singleton<LobbyManager<Player, Lobby>> {
public class LobbyManager : Singleton<LobbyManager> {
    public string PlayerName;
    protected const string DEFAULT_NAME = "Player";

    protected Lobby hostedLobby;
    protected Lobby joinedLobby;

    public const string KEY_PLAYER_NAME = "PlayerName";
    public const string KEY_RELAY_CODE = "RelayCode";

    public event Action OnLoggedIn;

    public event Action<Lobby> OnLobbyUpdate;

    [SerializeField]
    private ConnectionManager _connectionManagerPrefab;

    private void OnEnable() {
        NetworkManagerHooks.Instance.OnConnected += HandleConnected;
        NetworkManagerHooks.Instance.OnDisconnected += CleanUp;
    }

    private void OnDisable() {
        NetworkManagerHooks.Instance.OnConnected -= HandleConnected;
        NetworkManagerHooks.Instance.OnDisconnected -= CleanUp;
    }

    private void OnDestroy() {
        CleanUp();
    }

    protected virtual void HandleConnected() {
        if (!InstanceFinder.IsServer) {
            return;
        }

        GameObject connectionManager = Instantiate(_connectionManagerPrefab.gameObject);
        InstanceFinder.ServerManager.Spawn(connectionManager, null);
    }

    protected virtual void CleanUp() {
        if (!this.gameObject.scene.isLoaded) {
            return;
        }

        if (joinedLobby != null) {
            LeaveLobby();
        }
    }

    protected virtual void InvokeLoggedIn() {
        Debug.Log($"Logged in as {PlayerName}");
        OnLoggedIn?.Invoke();
    }

    public virtual void Authenticate(string givenPlayerName) {
        if (givenPlayerName != "") {
            PlayerName = givenPlayerName;
        } else {
            int randomId = UnityEngine.Random.Range(0, 10000);
            PlayerName = $"{DEFAULT_NAME}-{randomId}";
        }
    }

    public virtual bool IsHost() {
        return hostedLobby != null;
    }

    public virtual Task<List<Lobby>> GetLobbies() {
        return null;
    }

    protected virtual Player CreatePlayer() {
        return default(Player);
    }

    public virtual void JoinLobbyById(string lobbyId) {
    }

    protected void SetJoinedLobby(Lobby lobby) {
        UpdateLobby(lobby);
    }

    protected void UpdateLobby(Lobby lobby) {
        joinedLobby = lobby;
        OnLobbyUpdate?.Invoke(lobby);
    }

    public virtual void LeaveLobby() {
        if (joinedLobby != null) {
            hostedLobby = default(Lobby); // TODO check this is null
            SetJoinedLobby(default(Lobby));
        }
    }

    public virtual void MigrateLobbyHost() {
    }

    public virtual Task<bool> CreateLobby(string givenLobbyName) {
        return null;
    }

    public string GenerateLobbyName(string givenLobbyName) {
        return givenLobbyName != "" ? givenLobbyName : $"{PlayerName}'s Lobby";
    }
}