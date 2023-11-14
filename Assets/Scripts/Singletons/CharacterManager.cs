using FishNet.Object;
using System.Collections;
using UnityEngine;

public class CharacterManager : NetworkSingleton<CharacterManager> {

    // TEMP
    [SerializeField]
    private GameObject _characterPrefab;

    [ServerRpc(RequireOwnership = false)]
    public void SpawnCharacter(string characterId, Vector3 spawnPosition) {
        GameObject go = Instantiate(_characterPrefab);
        go.transform.position = spawnPosition;

        base.Spawn(go);
    }
}