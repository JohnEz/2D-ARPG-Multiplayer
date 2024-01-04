using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;

public struct SessionPlayerData {
    public string PlayerName;

    public SessionPlayerData(int clientID, string name, PersistentPlayer player, bool isConnected = false) {
        ClientID = clientID;
        PlayerName = name;
        IsConnected = isConnected;
        PersistentPlayer = player;
        IsLoadingScene = false;
    }

    public bool IsConnected { get; set; }

    public int ClientID { get; set; }

    public PersistentPlayer PersistentPlayer { get; set; }
    public bool IsLoadingScene { get; set; }
}

public class ConnectionManager : Singleton<ConnectionManager> {

    [SerializeField]
    private GameObject _serverConnectionManagerPrefab;

    public event Action<SessionPlayerData> OnPlayerConnected;

    public event Action<string> OnPlayerDisconnected;

    public event Action<SessionPlayerData> OnPlayerStartedLoadingScene;

    public event Action<SessionPlayerData> OnPlayerLoadedScene;

    private void OnEnable() {
        NetworkManagerHooks.Instance.OnServerConnected += HandleConnectedServer;
    }

    private void OnDisable() {
        NetworkManagerHooks.Instance.OnServerConnected -= HandleConnectedServer;
    }

    private void HandleConnectedServer() {
        if (!ServerConnectionManager.Instance) {
            // We spawn the serverConnectionManager when we start hosting the server so that we can set it as a global object
            // you can only set networked objects as global if they are spawned via code, not in the scene
            GameObject serverConnectionManager = Instantiate(_serverConnectionManagerPrefab);
            InstanceFinder.ServerManager.Spawn(serverConnectionManager);
        }
    }

    public void PlayerConnected(SessionPlayerData playerData) {
        OnPlayerConnected?.Invoke(playerData);
    }

    public void PlayerDisconnected(string playerId) {
        Debug.Log($"Player disconnected {playerId}");
        OnPlayerDisconnected?.Invoke(playerId);
    }

    public void PlayerStartedLoadingScene(SessionPlayerData playerData) {
        OnPlayerStartedLoadingScene?.Invoke(playerData);
    }

    public void PlayerLoadedScene(SessionPlayerData playerData) {
        OnPlayerLoadedScene?.Invoke(playerData);
    }

    public SyncDictionary<string, SessionPlayerData> GetConnections() {
        if (!ServerConnectionManager.Instance) {
            return null;
        }

        return ServerConnectionManager.Instance.Connections;
    }
}