using System.Collections;
using UnityEngine;

public class ChannelAbility : AbilityEffect {

    [SerializeField]
    private GameObject _channelVFXPrefab;

    [SerializeField]
    protected float _channelDuration = 1f;

    [SerializeField]
    private float _speedWhileChannelingMultiplier = 0.1f;

    public float SpeedWhileChannelingMultiplier { get { return _speedWhileChannelingMultiplier; } }

    private GameObject _channelVFX;

    [SerializeField]
    private AudioClip _channelSFX;

    public override void OnCastComplete(bool isOwner) {
        base.OnCastComplete(isOwner);

        _caster.GetComponent<ChannelController>().StartChanneling(_channelDuration, 0, this);
    }

    public virtual void OnChannelStart() {
        if (_channelVFXPrefab != null) {
            _channelVFX = Instantiate(_channelVFXPrefab, _caster.transform);
        }

        if (_channelSFX != null) {
            // TODO is there a way to loop this and stop when we stop channeling without just deleting the audio source?
            AudioManager.Instance.PlaySound(_channelSFX, _caster.transform);
        }
    }

    public virtual void OnChannelComplete() {
        Destroy(_channelVFX);
        Destroy(gameObject);
    }
}