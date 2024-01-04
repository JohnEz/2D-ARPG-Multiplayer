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

    public override void OnOwnershipClient(NetworkConnection prevOwner) {
        base.OnOwnershipClient(prevOwner);

        if (prevOwner == Owner) {
            Debug.Log("Tried setting persistent player owner to themself");
            return;
        }

        if (IsOwner) {
            Username = LobbyManager.Instance?.PlayerName ?? "Dev";
            HandleConnectedServer(Username);
        }
    }

    public void OnDestroy() {
        HandleDisconnect();
    }

    public override void OnStopNetwork() {
        HandleDisconnect();
    }

    private void HandleDisconnect() {
        if (!IsServer) {
            return;
        }

        ServerConnectionManager.Instance.PlayerDisconnectedServer(OwnerId);

        SessionPlayerData? existingSessionData = ServerConnectionManager.Instance.GetPlayerData(OwnerId);
        if (existingSessionData != null) {
            //save the session data
        }
    }

    [ServerRpc]
    private void HandleConnectedServer(string playerId) {
        if (!IsServer) {
            return;
        }

        ServerConnectionManager.Instance.PlayerConnectedServer(OwnerId, playerId, this);

        //existingSessionData = ConnectionManager.instance.GetPlayerData(OwnerId);

        //if (existingSessionData) {
        //set this player data to the stored value

        // if the character has already spawned
        // set this' character to that character

        // else
        // create a new character
        // update sessionData
        //}
    }

    [Server(Logging = LoggingType.Off)]
    public NetworkStats SpawnServer(Vector3 spawnLocation) {
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
            return null;
        }

        GameObject instance = Instantiate(prefab);
        instance.transform.position = spawnLocation;
        instance.GetComponent<CharacterController>().Username = Username; // TODO id like the persistent character or connection to hold this ownership
        InstanceFinder.ServerManager.Spawn(instance, Owner);

        return instance.GetComponent<NetworkStats>();
    }
}