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
    private Dictionary<string, PlayerTile> _playersTiles = new();

    [SerializeField]
    private Transform _listTransform;

    private void Start() {
        if (ConnectionManager.Instance == null) {
            return;
        }

        ConnectionManager.Instance
            .GetConnections()?.Values
            .ToList()
            .ForEach(playerData => {
                if (!playerData.IsConnected) {
                    return;
                }

                AddPlayerTile(playerData);
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

    private void AddPlayerTile(SessionPlayerData playerData) {
        Debug.Log("Adding player tile");
        PlayerTile tile = Instantiate(_playerTilePrefab, _listTransform);
        tile.SetPlayer(playerData.PersistentPlayer);

        _playersTiles[playerData.PlayerName] = tile;
    }

    private void RemovePlayer(string playerId) {
        if (!_playersTiles.ContainsKey(playerId)) {
            return;
        }

        PlayerTile tile = _playersTiles[playerId];
        Destroy(tile.gameObject);
        _playersTiles.Remove(playerId);
    }
}