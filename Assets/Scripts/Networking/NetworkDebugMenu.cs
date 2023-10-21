using FishNet;
using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using UnityEngine;

public class NetworkDebugMenu : NetworkBehaviour {

    [SerializeField]
    private NetworkDebugUI _debugUI;

    // TODO these should be stored somewhere else
    [SerializeField]
    private GameObject _magePrefab;

    [SerializeField]
    private GameObject _warriorPrefab;

    [SerializeField]
    private GameObject _alchemistPrefab;

    public override void OnOwnershipClient(NetworkConnection prevOwner) {
        base.OnOwnershipClient(prevOwner);

        if (IsOwner) {
            _debugUI.gameObject.SetActive(true);
        } else {
            Destroy(_debugUI.gameObject);
        }
    }

    [ServerRpc]
    public void SpawnServer(string character) {
        GameObject prefab = null;

        switch (character) {
            case "Mage":
                prefab = _magePrefab;
                break;

            case "Warrior":
                prefab = _warriorPrefab;
                break;

            case "Alchemist":
                prefab = _alchemistPrefab;
                break;
        }

        if (prefab == null) {
            return;
        }

        GameObject instance = Instantiate(prefab);
        InstanceFinder.ServerManager.Spawn(instance, Owner);

        Debug.Log($"Spawned {character} for {Owner.ClientId}");
        Debug.Log($"{Owner}");

        SpawnedObserver();
    }

    [ObserversRpc]
    public void SpawnedObserver() {
        _debugUI.gameObject.SetActive(false);
    }
}