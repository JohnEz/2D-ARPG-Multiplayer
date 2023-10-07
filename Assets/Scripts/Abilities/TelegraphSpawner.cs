using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;

public class TelegraphSpawner : NetworkBehaviour {

    /// <summary>
    /// Maximum amount of passed time a projectile may have.
    /// This ensures really laggy players won't be able to disrupt
    /// other players by having the projectile speed up beyond
    /// reason on their screens.
    /// </summary>
    private const float MAX_PASSED_TIME = 0.3f;

    public void Fire(string telegraphId, Vector3 position) {
        SpawnTelegraph(telegraphId, position, 0f);
        NotifyFire(telegraphId, position, base.TimeManager.Tick);
    }

    private void NotifyFire(string telegraphId, Vector3 position, uint tick) {
        if (InstanceFinder.IsServer) {
            ObserversFire(telegraphId, position, tick);
        } else {
            ServerFire(telegraphId, position, tick);
        }
    }

    private void SpawnTelegraph(string telegraphId, Vector3 position, float passedTime) {
        PredictedTelegraph telegraphPrefab = ResourceManager.Instance.GetTelegraph(telegraphId);
        PredictedTelegraph createdTelegraph = Instantiate(telegraphPrefab, position, Quaternion.identity);
        createdTelegraph.Initialise(passedTime, GetComponent<CharacterController>());
    }

    [ServerRpc]
    private void ServerFire(string telegraphId, Vector3 position, uint tick) {
        //Get passed time. Note the false for allow negative values.
        float passedTime = (float)base.TimeManager.TimePassed(tick, false);

        passedTime = Mathf.Min(MAX_PASSED_TIME / 2f, passedTime);

        SpawnTelegraph(telegraphId, position, passedTime);

        ObserversFire(telegraphId, position, tick);
    }

    [ObserversRpc(ExcludeOwner = true, ExcludeServer = true)]
    private void ObserversFire(string telegraphId, Vector3 position, uint tick) {
        float passedTime = (float)base.TimeManager.TimePassed(tick, false);
        passedTime = Mathf.Min(MAX_PASSED_TIME, passedTime);

        SpawnTelegraph(telegraphId, position, passedTime);
    }
}