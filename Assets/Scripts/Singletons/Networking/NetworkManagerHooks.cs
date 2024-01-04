using FishNet.Transporting;
using System;
using FishNet.Managing.Client;
using FishNet.Managing.Scened;
using FishNet.Managing.Server;

public class NetworkManagerHooks : Singleton<NetworkManagerHooks> {
    private ServerManager _serverManager;

    private ClientManager _clientManager;

    // Local hooks
    public event Action OnClientConnecting;

    public event Action OnClientConnected;

    public event Action OnLoadedNetworkObjects;

    public event Action OnClientDisconnecting;

    public event Action OnClientDisconnected;

    // server specific hooks
    public event Action OnServerConnected;

    private void Awake() {
        _serverManager = GetComponent<ServerManager>();
        _clientManager = GetComponent<ClientManager>();
    }

    private void OnEnable() {
        _serverManager.OnServerConnectionState += HandleServerStateChange;
        _clientManager.OnClientConnectionState += HandleClientStateChange;
    }

    private void OnDisable() {
        _serverManager.OnServerConnectionState -= HandleServerStateChange;
        _clientManager.OnClientConnectionState -= HandleClientStateChange;
    }

    protected void HandleServerStateChange(ServerConnectionStateArgs args) {
        if (args.ConnectionState == LocalConnectionState.Starting) {
            //OnServerConnecting?.Invoke();
            return;
        }

        if (args.ConnectionState == LocalConnectionState.Started) {
            OnServerConnected?.Invoke();
            return;
        }

        if (args.ConnectionState == LocalConnectionState.Stopping) {
            //OnServerDisconnecting?.Invoke();
            return;
        }

        if (args.ConnectionState == LocalConnectionState.Stopped) {
            //OnServerDisconnected?.Invoke();
            return;
        }
    }

    protected void HandleClientStateChange(ClientConnectionStateArgs args) {
        if (args.ConnectionState == LocalConnectionState.Starting) {
            OnClientConnecting?.Invoke();
            return;
        }

        if (args.ConnectionState == LocalConnectionState.Started) {
            OnClientConnected?.Invoke();
            return;
        }

        if (args.ConnectionState == LocalConnectionState.Stopping) {
            OnClientDisconnecting?.Invoke();
            return;
        }

        if (args.ConnectionState == LocalConnectionState.Stopped) {
            OnClientDisconnected?.Invoke();
            return;
        }
    }
}