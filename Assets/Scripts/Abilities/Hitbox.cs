using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(Collider2D))]
public class Hitbox : MonoBehaviour {
    private NetworkStats _caster;

    [SerializeField] private float hitDelay = 0f;
    [SerializeField] private float hitDuration = 0f;

    public event Action<Vector3, NetworkStats, NetworkStats> OnHit;

    private bool isActive;

    private List<int> hitIds;

    // TODO
    // add delay param
    // add duration param
    // add offset
    // add size

    public void Initialise(NetworkStats caster, float duration) {
        hitIds = new List<int>();
        _caster = caster;
        hitDuration = duration;

        Invoke("EnableHits", hitDelay);
    }

    private void EnableHits() {
        isActive = true;

        if (hitDuration > 0) {
            Invoke("DisableHits", hitDuration);
        }
    }

    private void DisableHits() {
        // TODO may not need this and can just destroy
        isActive = false;
        Destroy(gameObject, 1f);
    }

    public void OnTriggerEnter2D(Collider2D collision) {
        if (!isActive) {
            return;
        }

        OnCollision(collision);
    }

    public void OnTriggerStay2D(Collider2D collision) {
        if (!isActive) {
            return;
        }

        OnCollision(collision);
    }

    private void OnCollision(Collider2D collision) {
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

        bool shouldHitTarget = !hitCharacter || _caster.Faction != hitCharacter.Faction;

        if (!shouldHitTarget) {
            return;
        }

        if (hitCharacter) {
            HandleHit(hitCharacter, collision.gameObject.transform.position);
        }
    }

    private void HandleHit(NetworkStats hitCharacter, Vector3 hitPosition) {
        if (_caster.IsOwner) {
            CameraManager.Instance.ShakeCamera(2.5f, 0.2f);
        }

        OnHit?.Invoke(hitPosition, _caster, hitCharacter);
    }
}