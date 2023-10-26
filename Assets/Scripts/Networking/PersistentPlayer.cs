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

        if (!IsServer) {
            return;
        }

        // Check to see if we have player data in a session manager

        //if we do, set the data to the session data

        // if the character has already spawned
        // set this player as the owner of the character

        // if the character has not spawned
        // spawn the character
        // set this player as the owner of the character
    }

    public override void OnOwnershipClient(NetworkConnection prevOwner) {
        base.OnOwnershipClient(prevOwner);

        // TODO move all of this into a connection manager

        if (LobbyMenu.Instance != null) {
            if (base.IsOwner) {
                Username = LobbyManager.Instance.PlayerName;
            }

            LobbyMenu.Instance.ClientConnected(this);
        }

        // TODO hacky code until we get connectikon manager added
        if (!IsOwner) {
            Debug.Log("not owner");
            return;
        }

        if (GameStateManager.Instance == null) {
            Debug.Log("no game manager");
            return;
        }

        Debug.Log("Telling server to make my character");
        LocalPlayerConnected();
    }

    //TEMP
    [ServerRpc]
    public void LocalPlayerConnected() {
        GameStateManager.Instance.PlayerJoined(this);
    }

    public void OnDestroy() {
        RemovePersistentPlayer();
    }

    public override void OnStopNetwork() {
        RemovePersistentPlayer();
    }

    private void RemovePersistentPlayer() {
        if (LobbyMenu.Instance != null) {
            LobbyMenu.Instance.ClientDisconnected(this);
        }

        if (!IsServer) {
            return;
        }

        // update the session manager with the player data
    }

    [Server(Logging = LoggingType.Off)]
    public void SpawnServer(Vector3 spawnLocation) {
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
            return;
        }

        GameObject instance = Instantiate(prefab);
        instance.transform.position = spawnLocation;
        InstanceFinder.ServerManager.Spawn(instance, Owner);
    }
}