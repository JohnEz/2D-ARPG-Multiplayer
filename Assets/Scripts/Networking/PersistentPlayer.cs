using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

public class PersistentPlayer : NetworkBehaviour {

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

    public void Start() {
        LobbyMenu.Instance.ClientConnected(this);
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
}