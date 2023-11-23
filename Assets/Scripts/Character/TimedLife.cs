using FishNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedLife : MonoBehaviour {

    [SerializeField]
    private float _lifeDuration = 5f;

    private NetworkStats _networkStats;

    private void Awake() {
        _networkStats = GetComponent<NetworkStats>();

        if (InstanceFinder.IsServer) {
            Invoke("KillSelf", _lifeDuration);
        }
    }

    private void KillSelf() {
        _networkStats.ServerKill();
    }
}