using System.Collections;
using UnityEngine;

public class CourageousLeap : LeapAbility {
    private const string TELEGRAPH_ID = "CourageousLeapTelegraph";

    public override void OnCastComplete(bool isOwner) {
        base.OnCastComplete(isOwner);

        if (isOwner) {
            _caster.GetComponent<TelegraphSpawner>().Fire(TELEGRAPH_ID, _landingSpot.safeSpot);
        }
    }

    public override void OnLeapComplete() {
        if (_caster.IsOwner) {
            CameraManager.Instance.ShakeCamera(2.5f, 0.2f);
        }

        base.OnLeapComplete();
    }
}