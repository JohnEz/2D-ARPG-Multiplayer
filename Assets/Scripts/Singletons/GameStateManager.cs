using FishNet;
using FishNet.Managing.Logging;
using FishNet.Managing.Scened;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class GameStateManager : NetworkSingleton<GameStateManager> {
    private bool isGameOver = false;

    [SerializeField]
    private List<Transform> _spawnLocations;

    [SerializeField]
    private GameObject _defeatScreenPrefab;

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
        // TODO i think this first listener might be dangerous if the player hasnt loaded the scene and is reconnecting
        ConnectionManager.Instance.OnPlayerConnected += HandlePlayerLoadedScene;
        ConnectionManager.Instance.OnPlayerLoadedScene += HandlePlayerLoadedScene;
    }

    private void OnDisable() {
        if (!ConnectionManager.Instance) {
            //return;
        }

        ConnectionManager.Instance.OnPlayerConnected -= HandlePlayerLoadedScene;
        ConnectionManager.Instance.OnPlayerLoadedScene -= HandlePlayerLoadedScene;
    }

    private void HandlePlayerLoadedScene(SessionPlayerData playerData) {
        SpawnPlayer(playerData.PersistentPlayer);
    }

    [Server]
    public void DefeatServer() {
        if (isGameOver) {
            return;
        }

        isGameOver = true;

        DefeatClient();
    }

    [ObserversRpc]
    private void DefeatClient() {
        Instantiate(_defeatScreenPrefab);
    }

    [Server]
    public void VictoryServer() {
        if (isGameOver) {
            return;
        }

        isGameOver = true;

        NetworkSceneLoader.Instance.LoadScene("Town");
        VictoryClient();
    }

    [ObserversRpc]
    private void VictoryClient() {
        //Instantiate(_victoryScreenPrefab);
    }
}