using FishNet;
using FishNet.Managing.Logging;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        NetworkManagerHooks.Instance.OnPlayerLoaded += HandlePlayerLoaded;

        if (ConnectionManager.Instance) {
            ConnectionManager.Instance.OnPlayerLoadedScene += HandlePlayerLoadedScene;
        }
    }

    private void OnDisable() {
        if (NetworkManagerHooks.Instance != null) {
            NetworkManagerHooks.Instance.OnPlayerLoaded -= HandlePlayerLoaded;
        }

        if (ConnectionManager.Instance != null) {
            ConnectionManager.Instance.OnPlayerLoadedScene -= HandlePlayerLoadedScene;
        }
    }

    private void Start() {
        if (!ConnectionManager.Instance) {
            return;
        }

        Debug.Log("connections i can see " + ConnectionManager.Instance.Connections.Count);

        foreach (var item in ConnectionManager.Instance.Connections) {
            Debug.Log($"ClientId: {item.Key.ClientId}, IsLoadingScene:  {item.Value.IsLoadingScene}, PersistentPlayer:  {item.Value.PersistentPlayer != null}");

            if (InstanceFinder.IsServer && item.Value.PersistentPlayer != null && !item.Value.IsLoadingScene) {
                SpawnPlayer(item.Value.PersistentPlayer);
            }
        }
    }

    [Server(Logging = LoggingType.Off)]
    public void HandlePlayerLoaded(PersistentPlayer player) {
        Debug.Log("Player loaded");

        if (InstanceFinder.IsServer) {
            SpawnPlayer(player);
        }
    }

    [Server(Logging = LoggingType.Off)]
    private void HandlePlayerLoadedScene(PlayerConnectionState player) {
        Debug.Log("Player loaded scene");

        if (InstanceFinder.IsServer) {
            SpawnPlayer(player.PersistentPlayer);
        }
    }
}