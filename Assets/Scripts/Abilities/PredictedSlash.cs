using FishNet;
using FishNet.Managing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PredictedSlash : MonoBehaviour {

    public event Action<Vector3, NetworkStats, NetworkStats> OnHit;

    private NetworkStats _caster;

    [SerializeField] private float hitDelay = 0.1f;
    [SerializeField] private float hitDuration = 0.2f;

    [SerializeField]
    private AudioClip _spawnSFX;

    [SerializeField]
    private AudioClip _hitSFX;

    private bool isActive;

    private List<int> hitIds;

    public void Initialise(NetworkStats caster, Vector2 direction, float passedTime) {
        hitIds = new List<int>();
        _caster = caster;

        transform.SetParent(caster.transform);
        transform.localPosition = Vector3.zero;
        transform.up = direction;

        AudioClipOptions audioClipOptions = new AudioClipOptions();
        audioClipOptions.RandomPitch = true;
        audioClipOptions.PitchRange = 0.15f;

        AudioManager.Instance.PlaySound(_spawnSFX, transform.position, audioClipOptions);

        float calculatedDelay = hitDelay - passedTime;

        if (calculatedDelay > 0.025f) {
            Invoke("EnableHits", calculatedDelay);
        } else {
            EnableHits();
        }
    }

    private void EnableHits() {
        isActive = true;
        Invoke("DisableHits", hitDuration);
    }

    private void DisableHits() {
        isActive = false;
        Destroy(gameObject, 1f);
    }

    public void OnTriggerStay2D(Collider2D collision) {
        if (!isActive) {
            return;
        }

        bool hitUnit = collision.gameObject.tag == "Unit";

        if (!hitUnit) {
            return;
        }

        int hitId = collision.gameObject.GetInstanceID();
        if (hitIds.Contains(hitId)) {
            return;
        }

        hitIds.Add(hitId);

        NetworkStats hitCharacter = collision.gameObject.GetComponent<NetworkStats>();

        bool shouldHitTarget = hitCharacter && _caster.Faction != hitCharacter.Faction;

        if (!shouldHitTarget) {
            return;
        }

        if (hitCharacter) {
            HandleHit(hitCharacter, collision.gameObject.transform.position);
        }
    }

    private void HandleHit(NetworkStats hitCharacter, Vector3 hitPosition) {
        // ADD hit VFX
        //CreateHitEffect(hitPosition);

        AudioClipOptions audioClipOptions = new AudioClipOptions();
        audioClipOptions.RandomPitch = true;
        audioClipOptions.PitchRange = 0.15f;
        audioClipOptions.Volume = 0.75f;

        AudioManager.Instance.PlaySound(_hitSFX, transform.position, audioClipOptions);

        if (_caster.IsOwner) {
            CameraManager.Instance.ShakeCamera(1f, 0.1f);
        }

        OnHit?.Invoke(hitPosition, _caster, hitCharacter);
    }
}