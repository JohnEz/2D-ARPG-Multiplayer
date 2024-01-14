using FishNet;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class KillSelfEffect : AbilityEffect {

    public override void OnCastComplete(bool isOwner) {
        base.OnCastComplete(isOwner);

        base.OnCastComplete(isOwner);
        if (InstanceFinder.IsServer) {
            _caster.ServerKill();
        }

        Destroy(gameObject);
    }
}