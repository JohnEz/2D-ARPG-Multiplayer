using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

public class ServerConnectionManager : NetworkSingleton<ServerConnectionManager> {

    [SyncObject, SerializeField]
    public readonly SyncDictionary<string, SessionPlayerData> Connections = new();

    [SyncObject, SerializeField]
    private readonly SyncDictionary<int, string> _clientIDToPlayerId = new();

    private void OnEnable() {
        Connections.OnChange += HandleConnectionChange;

        EnableSceneEventListeners();
    }

    private void OnDisable() {
        Connections.OnChange -= HandleConnectionChange;

        DisableSceneEventListeners();
    }

    private void EnableSceneEventListeners() {
        if (!InstanceFinder.IsServer) {
            return;
        }

        InstanceFinder.SceneManager.OnClientPresenceChangeEnd += HandleClientPresenceChangeEnd;
    }

    private void DisableSceneEventListeners() {
        if (!InstanceFinder.IsServer) {
            return;
        }

        InstanceFinder.SceneManager.OnClientPresenceChangeEnd -= HandleClientPresenceChangeEnd;
    }

    private void SetupConnectingPlayerSessionData(int clientId, string playerId, SessionPlayerData playerData) {
        bool isReconnecting = false;

        if (IsDuplicateConnection(playerId)) {
            //    kickPlayer
            Debug.LogError("Player with the same playerid joined and is already connected");
            return;
        }

        if (Connections.ContainsKey(playerId)) {
            SessionPlayerData existingPlayerData = Connections[playerId];
            if (!existingPlayerData.IsConnected) {
                isReconnecting = true;
            }
        }

        if (isReconnecting) {
            Debug.Log($"Player {playerId} is reconnecting");
            playerData = Connections[playerId];
            playerData.ClientID = clientId;
            playerData.IsConnected = true;
        }

        _clientIDToPlayerId[clientId] = playerId;
        Connections[playerId] = playerData;
    }

    private void HandleDisconnectClient(int clientId) {
        string playerId = _clientIDToPlayerId[clientId];

        if (GetPlayerData(playerId)?.ClientID == clientId) {
            SessionPlayerData playerData = Connections[playerId];
            playerData.IsConnected = false;
            playerData.PersistentPlayer = null;
            Connections[playerId] = playerData;
        }
    }

    public bool IsDuplicateConnection(string playerId) {
        return Connections.ContainsKey(playerId) && Connections[playerId].IsConnected;
    }

    [Server]
    public void PlayerConnectedServer(int clientId, string playerId, PersistentPlayer player) {
        SetupConnectingPlayerSessionData(clientId, playerId, new SessionPlayerData(clientId, playerId, player, true));
    }

    [Server]
    public void PlayerDisconnectedServer(int clientId) {
        HandleDisconnectClient(clientId);
    }

    public SessionPlayerData? GetPlayerData(int clientId) {
        if (clientId == -1) {
            Debug.LogError($"Tried to get the player data for -1 (no owner)");
            return null;
        }

        string playerId = _clientIDToPlayerId[clientId];

        if (playerId == null) {
            Debug.LogError($"No client player ID found mapped to the given client ID: {clientId}");
            return null;
        }

        return GetPlayerData(playerId);
    }

    public SessionPlayerData? GetPlayerData(string playerId) {
        if (!Connections.ContainsKey(playerId)) {
            Debug.Log($"No PlayerData of matching player ID found: {playerId}");
            return null;
        }

        return Connections[playerId];
    }

    private void HandleConnectionChange(SyncDictionaryOperation operation, string key, SessionPlayerData value, bool asServer) {
        if (asServer && base.IsHost) {
            // if we are host we dont want to call it for server and client
            return;
        }

        if (operation == SyncDictionaryOperation.Add && value.IsConnected) {
            ConnectionManager.Instance.PlayerConnected(value);
            return;
        }

        if (operation == SyncDictionaryOperation.Remove && value.IsConnected) {
            ConnectionManager.Instance.PlayerDisconnected(key);
            return;
        }

        if (operation == SyncDictionaryOperation.Set) {
            // TODO check if already was connected
            if (value.IsConnected) {
                ConnectionManager.Instance.PlayerConnected(value);
            } else {
                ConnectionManager.Instance.PlayerDisconnected(key);
            }
        }

        Debug.Log($"operation: {operation}, connected: {value.IsConnected}");
    }

    private void HandleClientPresenceChangeEnd(ClientPresenceChangeEventArgs args) {
        if (!_clientIDToPlayerId.ContainsKey(args.Connection.ClientId)) {
            Debug.LogError($"Player not connected {args.Connection.ClientId}, trying to end loading scene");
            return;
        }

        string playerId = _clientIDToPlayerId[args.Connection.ClientId];
        SessionPlayerData playerData = Connections[playerId];

        if (args.Added) {
            Debug.Log($"Player loaded scene {playerId}");
            ConnectionManager.Instance.PlayerLoadedScene(playerData);
        } else {
            Debug.Log($"Player started loading scene {playerId}");
            ConnectionManager.Instance.PlayerStartedLoadingScene(playerData);
        }
    }
}