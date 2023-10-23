using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityEffect : MonoBehaviour {
    protected CharacterController _caster;

    [SerializeField]
    protected AudioClip _castCompleteSFX;

    [SerializeField]
    protected GameObject _castCompleteVFX;

    public void Initialise(CharacterController caster) {
        _caster = caster;
    }

    public virtual void OnCastStart() {
    }

    public virtual void OnCastComplete(bool isOwner) {
        if (_castCompleteVFX) {
            Instantiate(_castCompleteVFX, _caster.transform.position, Quaternion.identity);
        }

        AudioManager.Instance.PlaySound(_castCompleteSFX, _caster.transform.position);
    }
}