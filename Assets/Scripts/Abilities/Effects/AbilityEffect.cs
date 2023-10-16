using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityEffect : MonoBehaviour {
    protected CharacterController _caster;

    public void Initialise(CharacterController caster) {
        _caster = caster;
    }

    public virtual void OnCastStart() {
    }

    public virtual void OnCastComplete(bool isOwner) {
    }
}