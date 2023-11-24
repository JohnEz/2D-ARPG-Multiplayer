using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;

public class AbilitySpawner : NetworkBehaviour {

    /// <summary>
    /// Maximum amount of passed time a projectile may have.
    /// This ensures really laggy players won't be able to disrupt
    /// other players by having the projectile speed up beyond
    /// reason on their screens.
    /// </summary>
    private const float MAX_PASSED_TIME = 0.3f;

    public void CreatePersistentAOE(string persistentId, Vector3 position) {
        InstantiatePersistentAOE(persistentId, position, 0f);
        NotifyCreatePersistentAOE(persistentId, position, base.TimeManager.Tick);
    }

    private void NotifyCreatePersistentAOE(string persistentId, Vector3 position, uint tick) {
        if (InstanceFinder.IsServer) {
            ObserversPersistentAOE(persistentId, position, tick);
        } else {
            ServerPersistentAOE(persistentId, position, tick);
        }
    }

    private void InstantiatePersistentAOE(string persistentId, Vector3 position, float passedTime) {
        PersistentAOE persistentPrefab = ResourceManager.Instance.GetPersistentAOE(persistentId);
        PersistentAOE createdTelegraph = Instantiate(persistentPrefab, position, Quaternion.identity);
        createdTelegraph.Initialise(passedTime, GetComponent<NetworkStats>());
    }

    [ServerRpc]
    private void ServerPersistentAOE(string persistentId, Vector3 position, uint tick) {
        //Get passed time. Note the false for allow negative values.
        float passedTime = (float)base.TimeManager.TimePassed(tick, false);
        passedTime = Mathf.Min(MAX_PASSED_TIME / 2f, passedTime);

        InstantiatePersistentAOE(persistentId, position, passedTime);

        ObserversPersistentAOE(persistentId, position, tick);
    }

    [ObserversRpc(ExcludeOwner = true, ExcludeServer = true)]
    private void ObserversPersistentAOE(string persistentId, Vector3 position, uint tick) {
        float passedTime = (float)base.TimeManager.TimePassed(tick, false);
        passedTime = Mathf.Min(MAX_PASSED_TIME, passedTime);

        InstantiatePersistentAOE(persistentId, position, passedTime);
    }
}