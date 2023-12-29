using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Object.Synchronizing;
using System;
using FishNet;
using FishNet.Managing.Logging;

public class PersistentPlayer : NetworkBehaviour {

    // TODO the prefabs should be stored somewhere else
    [SerializeField]
    private GameObject _magePrefab;

    [SerializeField]
    private GameObject _warriorPrefab;

    [SerializeField]
    private GameObject _alchemistPrefab;

    [field: SyncVar(ReadPermissions = ReadPermission.ExcludeOwner, OnChange = nameof(HandleUsernameChanged))]
    public string Username { get; [ServerRpc(RunLocally = true)] set; }

    [field: SyncVar(ReadPermissions = ReadPermission.ExcludeOwner, OnChange = nameof(HandleSelectedCharacterChanged))]
    public string SelectedCharacter { get; [ServerRpc(RunLocally = true)] set; }

    [field: SyncVar(ReadPermissions = ReadPermission.ExcludeOwner, OnChange = nameof(HandleIsReadyChanged))]
    public bool IsReady { get; [ServerRpc(RunLocally = true)] set; }

    public event Action OnUsernameChanged;

    public event Action OnCharacterChanged;

    public event Action OnReadyStateChanged;

    private void HandleUsernameChanged(string oldValue, string newValue, bool asServer) {
        OnUsernameChanged?.Invoke();
    }

    private void HandleSelectedCharacterChanged(string oldValue, string newValue, bool asServer) {
        OnCharacterChanged?.Invoke();
    }

    private void HandleIsReadyChanged(bool oldValue, bool newValue, bool asServer) {
        OnReadyStateChanged?.Invoke();
    }

    public override void OnStartNetwork() {
        base.OnStartNetwork();

        // is the connection useful here?
        // is owner id persistent between connections?
        gameObject.name = $"Player {base.OwnerId}";
    }

    public override void OnStartClient() {
        base.OnStartClient();

        if (!base.IsOwner) {
            return;
        }

        if (LobbyManager.Instance != null) {
            Username = LobbyManager.Instance.PlayerName;
        }

        NetworkManagerHooks.Instance.HandleCompletedConnection();
    }

    public override void OnOwnershipClient(NetworkConnection prevOwner) {
        base.OnOwnershipClient(prevOwner);

        if (prevOwner == Owner) {
            return;
        }

        // Shouldnt be needed after connection manager changes, function should be remnoved?
        NetworkManagerHooks.Instance.HandlePlayerLoaded(this);

        if (prevOwner.IsValid) {
            ConnectionManager.Instance?.RemovePersistentPlayer(prevOwner, this);
        }

        if (IsOwner) {
            ConnectionManager.Instance?.AddPersistentPlayer(Owner, this);
        }
    }

    public void OnDestroy() {
        RemovePersistentPlayer();
    }

    public override void OnStopNetwork() {
        RemovePersistentPlayer();
    }

    private void RemovePersistentPlayer() {
        if (LobbyMenu.Instance != null) {
            //LobbyMenu.Instance.ClientDisconnected(this);
        }

        if (!IsServer) {
            return;
        }

        // update the session manager with the player data
    }

    [Server(Logging = LoggingType.Off)]
    public void SpawnServer(Vector3 spawnLocation) {
        Debug.Log("Spawning on server");
        GameObject prefab = null;

        switch (SelectedCharacter) {
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
            Debug.LogError($"No prefab found for character {SelectedCharacter}");
            return;
        }

        GameObject instance = Instantiate(prefab);
        instance.transform.position = spawnLocation;
        instance.GetComponent<CharacterController>().Username = Username; // TODO id like the persistent character or connection to hold this ownership
        InstanceFinder.ServerManager.Spawn(instance, Owner);
    }
}