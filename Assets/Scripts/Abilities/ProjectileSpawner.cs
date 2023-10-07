using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Transporting;

public class ProjectileSpawner : NetworkBehaviour {

    /// <summary>
    /// Maximum amount of passed time a projectile may have.
    /// This ensures really laggy players won't be able to disrupt
    /// other players by having the projectile speed up beyond
    /// reason on their screens.
    /// </summary>
    private const float MAX_PASSED_TIME = 0.3f;

    public void Fire(string projectileId, Vector3 position, Vector3 direction) {
        Vector3 normalisedDirection = direction.normalized;

        SpawnProjectile(projectileId, position, normalisedDirection, 0f);
        NotifyFire(projectileId, position, normalisedDirection, base.TimeManager.Tick);
    }

    private void NotifyFire(string projectileId, Vector3 position, Vector3 direction, uint tick) {
        if (InstanceFinder.IsServer) {
            ObserversFire(projectileId, position, direction, tick);
        } else {
            ServerFire(projectileId, position, direction, tick);
        }
    }

    private void SpawnProjectile(string projectileId, Vector3 position, Vector3 direction, float passedTime) {
        PredictedProjectile projectilePrefab = ResourceManager.Instance.GetProjectile(projectileId);
        PredictedProjectile createdProjectile = Instantiate(projectilePrefab, position, Quaternion.identity);
        // TODO i dont like passsing the character controller this way as im not sure the spawner will always be on the character
        createdProjectile.Initialise(direction, passedTime, GetComponent<CharacterController>());
    }

    [ServerRpc]
    private void ServerFire(string projectileId, Vector3 position, Vector3 direction, uint tick) {
        /* You may want to validate position and direction here.
         * How this is done depends largely upon your game so it
         * won't be covered in this guide. */

        //Get passed time. Note the false for allow negative values.
        float passedTime = (float)base.TimeManager.TimePassed(tick, false);
        /* Cap passed time at half of constant value for the server.
         * In our example max passed time is 300ms, so server value
         * would be max 150ms. This means if it took a client longer
         * than 150ms to send the rpc to the server, the time would
         * be capped to 150ms. This might sound restrictive, but that would
         * mean the client would have roughly a 300ms ping; we do not want
         * to punish other players because a laggy client is firing. */
        passedTime = Mathf.Min(MAX_PASSED_TIME / 2f, passedTime);

        SpawnProjectile(projectileId, position, direction, passedTime);

        ObserversFire(projectileId, position, direction, tick);
    }

    [ObserversRpc(ExcludeOwner = true, ExcludeServer = true)]
    private void ObserversFire(string projectileId, Vector3 position, Vector3 direction, uint tick) {
        float passedTime = (float)base.TimeManager.TimePassed(tick, false);
        passedTime = Mathf.Min(MAX_PASSED_TIME, passedTime);

        SpawnProjectile(projectileId, position, direction, passedTime);
    }
}