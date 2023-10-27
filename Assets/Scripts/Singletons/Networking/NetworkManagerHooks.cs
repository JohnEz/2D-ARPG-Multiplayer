using FishNet.Transporting;
using FishNet;
using System.Collections;
using UnityEngine;
using System;
using FishNet.Managing.Client;
using FishNet.Managing.Scened;
using FishNet.Connection;

public class NetworkManagerHooks : Singleton<NetworkManagerHooks> {
    private ClientManager _clientManager;

    private SceneManager _sceneManager;

    // Local hooks
    public event Action OnConnecting;

    public event Action OnConnected;

    public event Action OnLoadedNetworkObjects;

    public event Action OnDisconnecting;

    public event Action OnDisconnected;

    public event Action<PersistentPlayer> OnPlayerLoaded;

    public event Action<PersistentPlayer> OnPlayerDisconnected;

    // server specific hooks
    public event Action OnClientLeftScene;

    public event Action OnClientJoinedScene;

    private void Awake() {
        _clientManager = GetComponent<ClientManager>();
        _sceneManager = GetComponent<SceneManager>();
    }

    private void OnEnable() {
        //InstanceFinder.ServerManager.OnServerConnectionState += HandleServerStateChange;
        _clientManager.OnClientConnectionState += HandleClientStateChange;
        _sceneManager.OnClientPresenceChangeEnd += HandleClientPresenceChangeEnd;
    }

    private void OnDisable() {
        //InstanceFinder.ServerManager.OnServerConnectionState -= HandleServerStateChange;
        _clientManager.OnClientConnectionState -= HandleClientStateChange;
        _sceneManager.OnClientPresenceChangeEnd -= HandleClientPresenceChangeEnd;
    }

    protected void HandleClientStateChange(ClientConnectionStateArgs args) {
        if (args.ConnectionState == LocalConnectionState.Starting) {
            OnConnecting?.Invoke();
            return;
        }

        if (args.ConnectionState == LocalConnectionState.Started) {
            OnConnected?.Invoke();
            return;
        }

        if (args.ConnectionState == LocalConnectionState.Stopping) {
            OnDisconnecting?.Invoke();
            return;
        }

        if (args.ConnectionState == LocalConnectionState.Stopped) {
            OnDisconnected?.Invoke();
            return;
        }
    }

    private void HandleClientPresenceChangeEnd(ClientPresenceChangeEventArgs args) {
        if (args.Added) {
            OnClientLeftScene?.Invoke();
        } else {
            OnClientJoinedScene?.Invoke();
        }
    }

    // i couldn't find an actual action for this so this is a hack for now
    public void HandleCompletedConnection() {
        Debug.Log("HandleCompletedConnection");
        OnLoadedNetworkObjects?.Invoke();
    }

    // Another hack for when first joining the server - the connection manager should manage this
    public void HandlePlayerLoaded(PersistentPlayer player) {
        OnPlayerLoaded?.Invoke(player);
    }

    public void HandlePlayerDisconnected(PersistentPlayer player) {
        OnPlayerDisconnected?.Invoke(player);
    }
}