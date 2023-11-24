using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PersistentAOE))]
public class PersistentAOEHitEffect : MonoBehaviour {

    private void OnEnable() {
        GetComponent<PersistentAOE>().OnTick += HandleTick;
    }

    private void OnDisable() {
        GetComponent<PersistentAOE>().OnTick -= HandleTick;
    }

    protected virtual void HandleTick(NetworkStats caster, List<NetworkStats> hitTargets) {
    }
}