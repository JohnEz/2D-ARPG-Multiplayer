using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelegraphHitEffect : MonoBehaviour {
    private PredictedTelegraph _telegraph;

    private void Awake() {
        _telegraph = GetComponent<PredictedTelegraph>();
    }

    private void OnEnable() {
        _telegraph.OnHit += HandleHit;
        _telegraph.OnHitCharacter += HandleCharacterHit;
    }

    private void OnDisable() {
        _telegraph.OnHit -= HandleHit;
        _telegraph.OnHitCharacter -= HandleCharacterHit;
    }

    protected virtual void HandleHit(NetworkStats caster, List<NetworkStats> hitCharacters) {
    }

    protected virtual void HandleCharacterHit(NetworkStats caster, NetworkStats hitCharacter) {
    }
}