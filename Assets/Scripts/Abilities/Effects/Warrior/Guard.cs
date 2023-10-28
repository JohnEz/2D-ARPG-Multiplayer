using System.Collections;
using UnityEngine;

public class Guard : ChannelAbility {

    [SerializeField]
    private GameObject _reflectPrefab;

    private GameObject _reflect;

    public override void OnCastComplete(bool isOwner) {
        base.OnCastComplete(isOwner);
    }

    public override void OnChannelStart() {
        base.OnChannelStart();

        if (_reflectPrefab != null) {
            _reflect = Instantiate(_reflectPrefab, _caster.FacingTransform);
            _reflect.GetComponent<ProjectileReflector>().SetCaster(_caster);
        }
    }

    public override void OnChannelComplete() {
        base.OnChannelComplete();

        if (_reflect != null) {
            Destroy(_reflect);
        }
    }
}