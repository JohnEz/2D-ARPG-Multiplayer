using FishNet.Connection;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerList : MonoBehaviour {

    [SerializeField]
    private PlayerTile _playerTilePrefab;

    [SerializeField]
    private List<PlayerTile> _playersTiles = new List<PlayerTile>();

    [SerializeField]
    private Transform _listTransform;

    private void Start() {
        if (ConnectionManager.Instance == null) {
            return;
        }

        ConnectionManager.Instance.Connections.Values.ToList().ForEach(playerState => {
            AddPlayerTile(playerState);
        });
    }

    private void OnEnable() {
        if (ConnectionManager.Instance == null) {
            return;
        }

        ConnectionManager.Instance.OnPlayerConnected += AddPlayerTile;
        ConnectionManager.Instance.OnPlayerDisconnected += RemovePlayer;
    }

    private void OnDisable() {
        if (ConnectionManager.Instance == null) {
            return;
        }

        ConnectionManager.Instance.OnPlayerConnected -= AddPlayerTile;
        ConnectionManager.Instance.OnPlayerDisconnected -= RemovePlayer;
    }

    public void AddPlayerTile(PlayerConnectionState player) {
        PlayerTile tile = Instantiate(_playerTilePrefab, _listTransform);
        tile.SetPlayer(player.PersistentPlayer);

        _playersTiles.Add(tile);
    }

    public void RemovePlayer(PlayerConnectionState player) {
    }
}