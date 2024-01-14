using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLocations : Singleton<SpawnLocations> {

    [SerializeField]
    private List<Transform> _spawnLocations;

    private int spawnIndex = 0;

    public Transform GetNextSpawnLocation() {
        Transform spawnLocation = _spawnLocations[spawnIndex];
        spawnIndex = (spawnIndex + 1) % _spawnLocations.Count;

        return spawnLocation;
    }
}