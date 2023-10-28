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

    private void SpawnPlayer(PersistentPlayer player) {
        if (!InstanceFinder.IsServer) {
            return;
        }

        Debug.Log("Spawning player");

        player.SpawnServer(_spawnLocations[spawnIndex].position);
        spawnIndex = (spawnIndex + 1) % _spawnLocations.Count;
    }

    private void OnEnable() {
        NetworkManagerHooks.Instance.OnPlayerLoaded += HandlePlayerLoaded;
        ConnectionManager.Instance.OnPlayerLoadedScene += HandlePlayerLoadedScene;
    }

    private void OnDisable() {
        NetworkManagerHooks.Instance.OnPlayerLoaded -= HandlePlayerLoaded;
        ConnectionManager.Instance.OnPlayerLoadedScene -= HandlePlayerLoadedScene;
    }

    private void Start() {
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