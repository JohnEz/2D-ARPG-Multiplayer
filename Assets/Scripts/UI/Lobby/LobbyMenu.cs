using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Managing.Logging;
using FishNet.Managing.Scened;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LobbyMenu : Singleton<LobbyMenu> {
    private PersistentPlayer _localPlayer;

    public PersistentPlayer LocalPlayer { get { return _localPlayer; } set { SetLocalPlayer(value); } }

    [SerializeField]
    private NetworkManager _networkManager;

    [SerializeField]
    private List<LobbyPlayerCard> _playerCards;

    [SerializeField]
    private Dictionary<LobbyPlayerCard, PersistentPlayer> _connections;

    [SerializeField]
    private GameObject _startGameButton;

    private void Awake() {
        _connections = new Dictionary<LobbyPlayerCard, PersistentPlayer>();

        _playerCards.ForEach(card => {
            _connections.Add(card, null);
        });
    }

    private void OnEnable() {
        if (!ConnectionManager.Instance) {
            Debug.Log("LobbyMenu - There is no connection manager in the scene!");
            return;
        }

        ConnectionManager.Instance.OnPlayerConnected += HandleClientConnected;
        ConnectionManager.Instance.OnPlayerDisconnected += HandleClientDisconnected;
    }

    private void OnDisable() {
        if (!ConnectionManager.Instance) {
            return;
        }

        ConnectionManager.Instance.OnPlayerConnected -= HandleClientConnected;
        ConnectionManager.Instance.OnPlayerDisconnected -= HandleClientDisconnected;
    }

    private void HandleClientConnected(SessionPlayerData playerData) {
        LobbyPlayerCard card = GetNextAvailableCard();
        PersistentPlayer player = playerData.PersistentPlayer;

        if (player.IsOwner) {
            LocalPlayer = player;
        }

        if (card != null) {
            card.SetPlayer(player);
            _connections[card] = player;
        }
    }

    private void HandleClientDisconnected(string playerId) {
        if (LocalPlayer.Username == playerId) {
            LocalPlayer = null;
        }

        foreach (KeyValuePair<LobbyPlayerCard, PersistentPlayer> pair in _connections) {
            if (pair.Value.Username == playerId) {
                pair.Key.SetPlayer(null);
                _connections[pair.Key] = null;
                return;
            }
        }
    }

    private LobbyPlayerCard GetNextAvailableCard() {
        foreach (KeyValuePair<LobbyPlayerCard, PersistentPlayer> pair in _connections) {
            if (pair.Value == null) {
                return pair.Key;
            }
        }

        return null;
    }

    private void SetLocalPlayer(PersistentPlayer player) {
        _localPlayer = player;

        bool isStartButtonVisible = LocalPlayer != null && LocalPlayer.IsHost;

        _startGameButton.SetActive(isStartButtonVisible);
    }

    public void ClickReady() {
        if (!LocalPlayer) {
            return;
        }

        LocalPlayer.IsReady = !_localPlayer.IsReady;
    }

    public void ClickStartGame() {
        if (!LocalPlayer) {
            return;
        }

        if (!LocalPlayer.IsOwner) {
            return;
        }

        if (!LocalPlayer.IsReady) {
            LocalPlayer.IsReady = true;
        }

        if (!AreAllPlayersReady()) {
            // TODO something to say not all players are ready
            return;
        }

        // load the game scene
        LoadGame();
    }

    private bool AreAllPlayersReady() {
        List<PersistentPlayer> players = _connections.Values.ToList().FindAll(player => player != null);

        return players.TrueForAll(player => player.IsReady);
    }

    [Server(Logging = LoggingType.Off)]
    private void LoadGame() {
        NetworkSceneLoader.Instance.LoadScene("Town");
    }
}