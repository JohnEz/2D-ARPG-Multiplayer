using FishNet.Object;
using System.Collections;
using UnityEngine;

public class CharacterManager : NetworkSingleton<CharacterManager> {

    [ServerRpc(RequireOwnership = false)]
    public void SpawnCharacter(string characterId, Vector3 spawnPosition) {
        NetworkStats characterToSpawn = ResourceManager.Instance.GetSpawnableCharacter(characterId);

        NetworkStats go = Instantiate(characterToSpawn);
        go.transform.position = spawnPosition;

        base.Spawn(go.gameObject);
    }
}