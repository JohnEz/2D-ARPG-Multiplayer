using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(VisualEffect))]
public class PersistentAOE : MonoBehaviour {

    [SerializeField]
    public float _radius;

    [SerializeField]
    private float _duration;

    [SerializeField]
    private float _tickInterval;

    [SerializeField]
    private bool _canHitCaster;

    [SerializeField]
    private bool _canHitEnemies;

    [SerializeField]
    private bool _canHitAllies;

    private NetworkStats _caster;

    private VisualEffect _effect;

    [HideInInspector]
    public event Action<NetworkStats, List<NetworkStats>> OnTick;

    private void Awake() {
        _effect = GetComponent<VisualEffect>();

        _effect.SetFloat("Radius", _radius);
        _effect.SetFloat("Duration", _duration);
    }

    public void Initialise(float passedTime, NetworkStats caster) {
        _caster = caster;

        HandleTick();

        Invoke("HandleDurationComplete", _duration - passedTime);

        // TODO need to  check this isnt less than 0
        float timeUntilFirstTick = _tickInterval - passedTime;
        Invoke("Tick", timeUntilFirstTick);
    }

    private void Tick() {
        Invoke("Tick", _tickInterval);
        HandleTick();
    }

    private void HandleTick() {
        List<NetworkStats> hitCharacters = PredictedTelegraph.GetCircleHitTargets(transform.position, _radius, _caster, _canHitCaster, _canHitEnemies, _canHitAllies);

        OnTick?.Invoke(_caster, hitCharacters);
    }

    private void HandleDurationComplete() {
        CancelInvoke();

        Destroy(gameObject);
    }
}