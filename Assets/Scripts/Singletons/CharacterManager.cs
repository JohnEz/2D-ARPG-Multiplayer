using FishNet;
using FishNet.Object;
using System.Collections;
using UnityEngine;

public class CharacterManager : NetworkSingleton<CharacterManager> {

    public void SpawnCharacter(string characterId, Vector3 spawnPosition) {
        if (IsServer) {
            SpawnCharacterServer(characterId, spawnPosition);
        } else {
            SpawnCharacterServerRPC(characterId, spawnPosition);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnCharacterServerRPC(string characterId, Vector3 spawnPosition) {
        SpawnCharacterServer(characterId, spawnPosition);
    }

    private void SpawnCharacterServer(string characterId, Vector3 spawnPosition) {
        NetworkStats characterToSpawn = ResourceManager.Instance.GetSpawnableCharacter(characterId);

        NetworkStats go = Instantiate(characterToSpawn);
        go.transform.position = spawnPosition;

        base.Spawn(go.gameObject);
    }
}