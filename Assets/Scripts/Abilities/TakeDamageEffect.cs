using System.Collections;
using UnityEngine;

public class TakeDamageEffect : AbilityEffect {

    [SerializeField]
    private int _damage = 2;

    public override void OnCastComplete(bool isOwner) {
        base.OnCastComplete(isOwner);

        _caster.GetComponent<NetworkStats>().TakeDamage(_damage, isOwner, _caster);

        Destroy(gameObject);
    }
}