using FishNet;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    protected Vector2 _direction;

    protected NetworkStats _caster;

    protected Rigidbody2D _body;

    [SerializeField]
    private GameObject visuals;

    protected bool isActive = true;

    public event Action<Vector3, NetworkStats, NetworkStats> OnHit;

    public event Action<Vector3, NetworkStats> OnHitLocation;

    // Config variables
    /////////////////////

    public bool CanHitAllies = false;

    public bool CanHitCaster = false;

    public bool CanHitEnemies = true;

    // FX

    [SerializeField]
    private AudioClip onCreateSFX;

    [SerializeField]
    private GameObject onHitVFXPrefab;

    [SerializeField]
    private List<AudioClip> onHitSFXs;

    private void Awake() {
        _body = GetComponent<Rigidbody2D>();
    }

    public virtual void Initialise(Vector2 direction, float passedTime, NetworkStats caster, float distance = -1f) {
        _caster = caster;
        _direction = direction;
        transform.up = direction;

        if (onCreateSFX) {
            AudioManager.Instance.PlaySound(onCreateSFX, transform);
        }
    }

    public virtual void InitialiseTargetLocation(Vector3 targetLocation, float passedTime, NetworkStats caster) {
        float distance = Vector2.Distance(transform.position, targetLocation);
        Vector2 direction = (targetLocation - transform.position).normalized;

        Initialise(direction, passedTime, caster, distance);
    }

    protected void CallOnHit(Vector3 location, NetworkStats hitCharacter) {
        OnHit?.Invoke(location, _caster, hitCharacter);
    }

    public void HandleHit(Vector3 hitLocation, bool isExpired = false) {
        CreateHitEffects(hitLocation, isExpired);

        OnHitLocation?.Invoke(hitLocation, _caster);

        // we destroy the visuals so the trail isnt instantly destroyed.
        if (visuals) {
            Destroy(visuals);
        }
        isActive = false;

        Destroy(gameObject, 0.2f);
    }

    public void CreateHitEffects(Vector3 hitLocation, bool isExpired) {
        if (!InstanceFinder.IsClient) {
            return;
        }

        if (onHitVFXPrefab) {
            GameObject hitVFX = Instantiate(onHitVFXPrefab);
            hitVFX.transform.position = hitLocation;
            hitVFX.transform.rotation = transform.rotation;
        }

        if (onHitSFXs.Count > 0) {
            AudioClipOptions options = new AudioClipOptions();

            if (isExpired) {
                options.Volume = 0.5f;
                options.Pitch = 0.75f;
            }

            AudioManager.Instance.PlaySound(onHitSFXs, transform.position, options);
        }
    }

    public bool ShouldReflect(NetworkStats reflector) {
        bool hitsUnits = CanHitAllies || CanHitEnemies || CanHitCaster;

        if (!hitsUnits) {
            return false;
        }

        if (reflector.Faction == _caster.Faction) {
            // cant reflect allies projectiles
            return false;
        }

        return true;
    }

    public void Reflect(Vector3 reflectPosition, NetworkStats _newCaster) {
        Vector2 reflectDirection = _direction * -1;
        Initialise(reflectDirection, 0f, _newCaster);
    }
}