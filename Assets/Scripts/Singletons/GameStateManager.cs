using FishNet;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameStateManager : Singleton<GameStateManager> {

    [SerializeField]
    private List<Transform> _spawnLocations;

    private int spawnIndex = 0;

    public void PlayerJoined(PersistentPlayer player) {
        if (!InstanceFinder.IsServer) {
            return;
        }

        player.SpawnServer(_spawnLocations[spawnIndex].position);
        spawnIndex = (spawnIndex + 1) % _spawnLocations.Count;
    }

    private void Start() {
        Debug.Log("GameStateManager Start()");

        List<PersistentPlayer> persistentPlayers = FindObjectsOfType<PersistentPlayer>().ToList();

        //persistentPlayers.ForEach(player => {
        //    Debug.Log("player exists already");
        //    PlayerJoined(player);
        //});

        // TODO rethink this

        persistentPlayers.ForEach(player => {
            if (player.IsOwner) {
                player.LocalPlayerConnected();
            }
        });
    }
}