using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyMenu : Singleton<LobbyMenu> {

    [SerializeField]
    private NetworkManager _networkManager;

    [SerializeField]
    private List<LobbyPlayerCard> _playerCards;

    [SerializeField]
    private Dictionary<LobbyPlayerCard, PersistentPlayer> _connections;

    private void Awake() {
        _connections = new Dictionary<LobbyPlayerCard, PersistentPlayer>();

        _playerCards.ForEach(card => {
            _connections.Add(card, null);
        });
    }

    //public void ClientConnected(NetworkConnection connection) {
    //    HandleClientConnected(connection);
    //}

    //public void ClientDisconnected(NetworkConnection connection) {
    //    HandleClientDisconnected(connection);
    //}

    public void ClientConnected(PersistentPlayer player) {
        HandleClientConnected(player);
    }

    public void ClientDisconnected(PersistentPlayer player) {
        HandleClientDisconnected(player);
    }

    private void HandleClientConnected(PersistentPlayer player) {
        LobbyPlayerCard card = GetNextAvailableCard();

        Debug.Log($"Player {player.OwnerId} connected");

        if (card != null) {
            Debug.Log($"Player {player.OwnerId} connected and assigned to card {card.name}");

            card.SetPlayer(player);
            _connections[card] = player;
        }
    }

    private void HandleClientDisconnected(PersistentPlayer player) {
        foreach (KeyValuePair<LobbyPlayerCard, PersistentPlayer> pair in _connections) {
            if (pair.Value == player) {
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
}