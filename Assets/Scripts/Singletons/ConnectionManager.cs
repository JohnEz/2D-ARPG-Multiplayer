using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerConnectionState {
    public PersistentPlayer PersistentPlayer { get; set; }
    public bool IsConnected { get; set; }
    public bool IsLoadingScene { get; set; }
}

public class ConnectionManager : NetworkSingleton<ConnectionManager> {

    [SyncObject, SerializeField]
    public readonly SyncDictionary<NetworkConnection, PlayerConnectionState> Connections = new();

    public event Action<PlayerConnectionState> OnPlayerStartedLoadingScene;

    public event Action<PlayerConnectionState> OnPlayerLoadedScene;

    private void OnEnable() {
        InstanceFinder.ServerManager.OnRemoteConnectionState += HandleRemoteConnectionChange;

        EnableSceneEventListeners();
    }

    private void OnDisable() {
        InstanceFinder.ServerManager.OnRemoteConnectionState -= HandleRemoteConnectionChange;

        DisableSceneEventListeners();
    }

    private void EnableSceneEventListeners() {
        if (!InstanceFinder.IsServer) {
            return;
        }
        InstanceFinder.SceneManager.OnLoadStart += HandleSceneLoadStart;
        InstanceFinder.SceneManager.OnClientPresenceChangeEnd += HandleClientPresenceChangeEnd;
    }

    private void DisableSceneEventListeners() {
        if (!InstanceFinder.IsServer) {
            return;
        }
        InstanceFinder.SceneManager.OnLoadStart -= HandleSceneLoadStart;
        InstanceFinder.SceneManager.OnClientPresenceChangeEnd -= HandleClientPresenceChangeEnd;
    }

    private void HandleRemoteConnectionChange(NetworkConnection connection, RemoteConnectionStateArgs args) {
        if (args.ConnectionState == RemoteConnectionState.Started) {
            PlayerConnected(connection);
        } else {
            PlayerDisconnected(connection);
        }
    }

    // not ran on server?
    private void HandleSceneLoadStart(SceneLoadStartEventArgs args) {
        //if (!InstanceFinder.IsServer) {
        //    Debug.Log("Client started loading scene but wasnt server");
        //    return;
        //}

        //if (Connections.ContainsKey(args.Connection)) {
        //    Debug.Log($"Player {args.Connection.ClientId} started loading scene {args.Scene.name}");
        //    Connections[args.Connection] = new PlayerConnectionState() {
        //        PersistentPlayer = Connections[args.Connection].PersistentPlayer,
        //        IsConnected = Connections[args.Connection].IsConnected,
        //        IsLoadingScene = true
        //    };
        //} else {
        //    Debug.LogError($"Player not connected {args.Connection.ClientId}, trying to start loading scene");
        //}
    }

    private void PlayerConnected(NetworkConnection connection) {
        if (Connections.ContainsKey(connection)) {
            Debug.LogError($"Player already connected {connection.ClientId}");
            return;
        }

        Debug.Log($"Player {connection.ClientId} connected");
        Connections.Add(connection, new PlayerConnectionState() {
            PersistentPlayer = null,
            IsConnected = true,
            IsLoadingScene = false
        });

        Connections.Dirty(connection);
    }

    private void PlayerDisconnected(NetworkConnection connection) {
        if (!Connections.ContainsKey(connection)) {
            Debug.LogError($"Player not connected {connection.ClientId} when trying to disconnect");
            return;
        }

        Debug.Log($"Player {connection.ClientId} disconnected");
        Connections.Remove(connection);
    }

    private void HandleClientPresenceChangeEnd(ClientPresenceChangeEventArgs args) {
        if (!Connections.ContainsKey(args.Connection)) {
            Debug.LogError($"Player not connected {args.Connection.ClientId}, trying to end loading scene");
        }

        Debug.Log($"Player {args.Connection.ClientId} changed presence to {args.Added}");
        Connections[args.Connection] = new PlayerConnectionState() {
            PersistentPlayer = Connections[args.Connection].PersistentPlayer,
            IsConnected = Connections[args.Connection].IsConnected,
            IsLoadingScene = !args.Added
        };

        if (args.Added) {
            OnPlayerLoadedScene?.Invoke(Connections[args.Connection]);
        } else {
            OnPlayerStartedLoadingScene?.Invoke(Connections[args.Connection]);
        }

        Connections.Dirty(args.Connection);
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddPersistentPlayer(NetworkConnection connection, PersistentPlayer player) {
        if (!Connections.ContainsKey(connection)) {
            Debug.LogError($"Player not connected {connection.ClientId} when trying to add persistent player");
            return;
        }

        Debug.Log($"Player {connection.ClientId} added persistent player {player.OwnerId}");

        Connections[connection] = new PlayerConnectionState() {
            PersistentPlayer = player,
            IsConnected = Connections[connection].IsConnected,
            IsLoadingScene = Connections[connection].IsLoadingScene,
        };

        Connections.Dirty(connection);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RemovePersistentPlayer(NetworkConnection connection, PersistentPlayer player) {
        if (!Connections.ContainsKey(connection)) {
            Debug.LogError($"Player not connected {connection.ClientId} when trying to remove persistent player");
            return;
        }

        if (Connections[connection].PersistentPlayer != player) {
            Debug.LogError($"Player {connection.ClientId} trying to remove persistent player that is not theirs");
            return;
        }

        Debug.Log($"Player {connection.ClientId} removing persistent player");

        Connections[connection] = new PlayerConnectionState() {
            PersistentPlayer = null,
            IsConnected = Connections[connection].IsConnected,
            IsLoadingScene = Connections[connection].IsLoadingScene,
        };

        Connections.Dirty(connection);
    }
}