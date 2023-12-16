using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PersistentAOE))]
public class PersistentAOEHitEffect : MonoBehaviour {

    [SerializeField]
    private GameObject _characterHitVfx;

    private void OnEnable() {
        GetComponent<PersistentAOE>().OnTick += HandleTick;
    }

    private void OnDisable() {
        GetComponent<PersistentAOE>().OnTick -= HandleTick;
    }

    protected virtual void HandleTick(NetworkStats caster, List<NetworkStats> hitTargets) {
        hitTargets.ForEach(hitTarget => {
            if (_characterHitVfx) {
                GameObject hitVFX = Instantiate(_characterHitVfx, hitTarget.transform);
                hitVFX.transform.position = hitTarget.transform.position;
            }
        });
    }
}