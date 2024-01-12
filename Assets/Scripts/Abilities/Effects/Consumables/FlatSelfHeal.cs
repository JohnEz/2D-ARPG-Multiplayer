using FishNet;
using System.Collections.Generic;
using UnityEngine;

public class FlatSelfHeal : AbilityEffect {

    [SerializeField]
    private int HEALING = 1;

    [SerializeField]
    private GameObject _characterHitVfx;

    [SerializeField]
    private bool isTrueHealing = false;

    public override void OnCastComplete(bool isOwner) {
        base.OnCastComplete(isOwner);

        _caster.ReceiveHealing(gameObject.name, _caster, HEALING, isTrueHealing);

        if (_characterHitVfx) {
            GameObject hitVFX = Instantiate(_characterHitVfx, _caster.transform);
            hitVFX.transform.position = _caster.transform.position;
        }

        Destroy(gameObject);
    }
}