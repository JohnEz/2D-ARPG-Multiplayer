using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityEffect : MonoBehaviour {
    protected NetworkStats _caster;

    protected CharacterController _casterController;

    [SerializeField]
    protected AudioClip _castCompleteSFX;

    [SerializeField]
    protected GameObject _castCompleteVFX;

    [SerializeField]
    protected AbilityEffect _nextAbilityEffect;

    public void Initialise(NetworkStats caster) {
        _caster = caster;
        _casterController = caster.GetComponent<CharacterController>();
    }

    public virtual void OnCastStart() {
    }

    public virtual void OnCastComplete(bool isOwner) {
        if (_castCompleteVFX) {
            Instantiate(_castCompleteVFX, _caster.transform.position, Quaternion.identity);
        }

        AudioManager.Instance.PlaySound(_castCompleteSFX, _caster.transform.position);

        if (_nextAbilityEffect) {
            AbilityEffect nextEffect = Instantiate(_nextAbilityEffect, _caster.transform.position, Quaternion.identity);
            nextEffect.Initialise(_caster);
            nextEffect.OnCastComplete(isOwner);
        }
    }
}