using System.Collections;
using UnityEngine;

public class Guard : ChannelAbility {

    [SerializeField]
    private GameObject _reflectPrefab;

    private GameObject _reflect;

    private const string BUFF = "Guard";

    public override void OnCastComplete(bool isOwner) {
        base.OnCastComplete(isOwner);
    }

    public override void OnChannelStart() {
        base.OnChannelStart();

        if (_reflectPrefab != null) {
            _reflect = Instantiate(_reflectPrefab, _caster.FacingTransform);
            _reflect.GetComponent<ProjectileReflector>().SetCaster(_caster);
        }

        _caster.GetComponent<BuffController>().ServerApplyBuff(BUFF);
    }

    public override void OnChannelComplete() {
        base.OnChannelComplete();

        BuffController buffController = _caster.GetComponent<BuffController>();

        if (buffController.HasBuff(BUFF)) {
            buffController.ServerRemoveBuff(BUFF);
        }

        if (_reflect != null) {
            Destroy(_reflect);
        }
    }
}