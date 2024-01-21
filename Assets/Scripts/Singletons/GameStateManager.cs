using FishNet;
using FishNet.Managing;
using FishNet.Object;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : NetworkSingleton<GameStateManager> {
    private bool isGameStarted = false;

    public bool IsGameStarted { get { return isGameStarted; } }

    private bool isGameOver = false;

    public bool IsGameOver { get { return isGameOver; } }

    [SerializeField]
    private GameObject _defeatScreenPrefab;

    private List<NetworkStats> _players = new();

    public List<NetworkStats> Players { get { return _players; } }

    private List<NetworkStats> _enemies = new();

    public List<NetworkStats> Enemies { get { return _enemies; } }

    private void OnEnable() {
        if (!InstanceFinder.IsServer) {
            return;
        }

        // TODO i think this first listener might be dangerous if the player hasnt loaded the scene and is reconnecting
        ConnectionManager.Instance.OnPlayerConnected += HandlePlayerLoadedScene;
        // TODO we are now loading multiple scenes at the same time and this is causing issues, maybe i only fire the event when the main scene loads
        ConnectionManager.Instance.OnPlayerLoadedScene += HandlePlayerLoadedScene;

        NetworkSceneLoader.Instance.OnSceneLoadStart += HandleLoadSceneStart;
    }

    private void OnDisable() {
        if (!InstanceFinder.IsServer) {
            return;
        }

        if (ConnectionManager.Instance) {
            ConnectionManager.Instance.OnPlayerConnected -= HandlePlayerLoadedScene;
            ConnectionManager.Instance.OnPlayerLoadedScene -= HandlePlayerLoadedScene;
        }

        if (NetworkSceneLoader.Instance) {
            NetworkSceneLoader.Instance.OnSceneLoadStart -= HandleLoadSceneStart;
        }
    }

    public void RegisterEnemy(NetworkStats enemy) {
        if (_enemies.Contains(enemy)) {
            return;
        }

        _enemies.Add(enemy);
    }

    public void UnregisterEnemy(NetworkStats enemy) {
        if (!_enemies.Contains(enemy)) {
            return;
        }

        _enemies.Remove(enemy);
    }

    private void SpawnPlayer(PersistentPlayer persistentPlayer) {
        if (!InstanceFinder.IsServer) {
            return;
        }

        if (!persistentPlayer) {
            Debug.LogError("No persistent player given to SpawnPlayer");
            return;
        }

        var existingPlayer = _players.Find(player => player.OwnerId == persistentPlayer.OwnerId);

        if (existingPlayer) {
            Debug.Log("Player already has a spawned character");
            return;
        }

        NetworkStats spawnedPlayer = persistentPlayer.SpawnServer(SpawnLocations.Instance.GetNextSpawnLocation().position);

        _players.Add(spawnedPlayer);

        // TODO this is a hack, we say the game has started when the first player spawns
        if (!IsGameStarted) {
            isGameStarted = true;
        }
    }

    private void HandleLoadSceneStart() {
        _players.Clear();
        isGameStarted = false;
        isGameOver = false;
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

    [ObserversRpc(RunLocally = true)]
    private void DefeatClient() {
        Debug.Log("Spawning defeat screen");
        Instantiate(_defeatScreenPrefab);
    }

    [Server]
    public void VictoryServer() {
        if (isGameOver) {
            return;
        }

        isGameOver = true;

        NetworkSceneLoader.Instance.LoadGameLevel("Town");
        VictoryClient();
    }

    [ObserversRpc]
    private void VictoryClient() {
        //Instantiate(_victoryScreenPrefab);
    }
}