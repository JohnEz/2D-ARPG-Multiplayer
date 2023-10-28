using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChannelController : MonoBehaviour {
    private CharacterStateController _stateController;

    public event Action<float, float> OnChannelStart;

    public event Action OnChannelComplete;

    public event Action OnChannelCancel;

    private ChannelAbility _channelingEffect;

    private void Awake() {
        _stateController = GetComponentInParent<CharacterStateController>();
    }

    public void StartChanneling(float duration, float passedTime, ChannelAbility channelingEffect) {
        _stateController.State = CharacterState.Channeling;

        _channelingEffect = channelingEffect;
        channelingEffect.OnChannelStart();

        OnChannelStart?.Invoke(duration, passedTime);

        Invoke("ChannelComplete", duration - passedTime);
    }

    public void StopChanneling() {
        EndChannel();

        OnChannelCancel?.Invoke();
    }

    public void ChannelComplete() {
        EndChannel();

        OnChannelComplete?.Invoke();
    }

    private void EndChannel() {
        _channelingEffect.OnChannelComplete();
        _channelingEffect = null;
        _stateController.State = CharacterState.Idle;
    }
}