using FishNet;
using FishNet.Managing.Logging;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class GameStateManager : Singleton<GameStateManager> {

    [SerializeField]
    private List<Transform> _spawnLocations;

    private int spawnIndex = 0;

    private List<NetworkStats> _players = new();

    public List<NetworkStats> Players { get { return _players; } }

    private void SpawnPlayer(PersistentPlayer player) {
        if (!InstanceFinder.IsServer) {
            return;
        }

        if (!player) {
            Debug.LogError("No persistent player given to SpawnPlayer");
            return;
        }

        NetworkStats spawnedPlayer = player.SpawnServer(_spawnLocations[spawnIndex].position);
        spawnIndex = (spawnIndex + 1) % _spawnLocations.Count;

        _players.Add(spawnedPlayer);
    }

    private void OnEnable() {
        if (!ConnectionManager.Instance) {
            return;
        }

        ConnectionManager.Instance.OnPlayerLoadedScene += HandlePlayerLoadedScene;
    }

    private void OnDisable() {
        if (!ConnectionManager.Instance) {
            return;
        }

        ConnectionManager.Instance.OnPlayerLoadedScene -= HandlePlayerLoadedScene;
    }

    private void HandlePlayerLoadedScene(SessionPlayerData playerData) {
        SpawnPlayer(playerData.PersistentPlayer);
    }
}