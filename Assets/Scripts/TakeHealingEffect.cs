using System.Collections;
using UnityEngine;

public class TakeHealingEffect : AbilityEffect {

    [SerializeField]
    private int _healing = 2;

    public override void OnCastComplete(bool isOwner) {
        base.OnCastComplete(isOwner);

        _caster.GetComponent<NetworkStats>().ReceiveHealing(_healing);

        Destroy(gameObject);
    }
}