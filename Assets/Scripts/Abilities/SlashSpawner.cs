using FishNet.Object;
using FishNet;
using System.Collections;
using UnityEngine;

public class SlashSpawner : NetworkBehaviour {
    private const float MAX_PASSED_TIME = 0.3f;

    public void Slash(string slashId, Vector3 startPosition, Vector3 direction) {
        Vector3 normalisedDirection = direction.normalized;

        SpawnSlash(slashId, startPosition, normalisedDirection, 0f);
        NotifySlash(slashId, startPosition, normalisedDirection, base.TimeManager.Tick);
    }

    private void NotifySlash(string slashId, Vector3 position, Vector3 direction, uint tick) {
        if (InstanceFinder.IsServer) {
            ObserversSlash(slashId, position, direction, tick);
        } else {
            ServerSlash(slashId, position, direction, tick);
        }
    }

    private void SpawnSlash(string slashId, Vector3 position, Vector3 direction, float passedTime) {
        PredictedSlash slashPrefab = ResourceManager.Instance.GetSlash(slashId);
        PredictedSlash createdSlash = Instantiate(slashPrefab, position, Quaternion.identity);
        // TODO i dont like passsing the character controller this way as im not sure the spawner script will always be on the character
        createdSlash.Initialise(GetComponent<NetworkStats>(), direction, passedTime);
    }

    [ServerRpc]
    private void ServerSlash(string slashId, Vector3 position, Vector3 direction, uint tick) {
        float passedTime = (float)base.TimeManager.TimePassed(tick, false);
        passedTime = Mathf.Min(MAX_PASSED_TIME / 2f, passedTime);

        SpawnSlash(slashId, position, direction, passedTime);

        ObserversSlash(slashId, position, direction, tick);
    }

    [ObserversRpc(ExcludeOwner = true, ExcludeServer = true)]
    private void ObserversSlash(string slashId, Vector3 position, Vector3 direction, uint tick) {
        float passedTime = (float)base.TimeManager.TimePassed(tick, false);
        passedTime = Mathf.Min(MAX_PASSED_TIME, passedTime);

        SpawnSlash(slashId, position, direction, passedTime);
    }
}