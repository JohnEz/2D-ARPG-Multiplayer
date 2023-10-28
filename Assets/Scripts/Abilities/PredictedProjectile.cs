using FishNet;
using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredictedProjectile : MonoBehaviour {

    // used to make sure the projectile doesnt last forever
    private const float MAX_LIFE_TIME = 5f;

    private const float MAX_DISTANCE_MODIFIER = -1f;

    private Vector2 _direction;
    private float _passedTime = 0f;

    private CharacterController _caster;

    private Rigidbody2D _body;

    [SerializeField]
    private GameObject visuals;

    private bool isActive = true;

    public event Action<Vector3, NetworkStats, NetworkStats> OnHit;

    private Vector2 _startPosition;

    // Config variables
    /////////////////////

    public float Speed = 15f;

    [SerializeField]
    private float _baseMaxDistance = 12f;

    public float MaxDistance {
        get {
            return _baseMaxDistance + MAX_DISTANCE_MODIFIER;
        }
    }

    public bool CanHitAllies = false;

    public bool CanHitCaster = false;

    public bool CanHitEnemies = true;

    // FX

    [SerializeField]
    private AudioClip onCreateSFX;

    [SerializeField]
    private GameObject onHitVFXPrefab;

    [SerializeField]
    private AudioClip onHitSFX;

    private void Awake() {
        _body = GetComponent<Rigidbody2D>();
    }

    public void Initialise(Vector2 direction, float passedTime, CharacterController caster) {
        _direction = direction.normalized;
        transform.up = direction;
        _passedTime = passedTime;
        _caster = caster; // TODO this should be network stats
        _startPosition = transform.position;

        Invoke("Expire", MAX_LIFE_TIME);

        if (onCreateSFX) {
            AudioManager.Instance.PlaySound(onCreateSFX, transform);
        }
    }

    private void Update() {
        if (!isActive) {
            return;
        }

        if (Vector2.Distance(transform.position, _startPosition) >= MaxDistance) {
            Expire();
        }
    }

    private void FixedUpdate() {
        if (!isActive) {
            return;
        }

        Move();
    }

    private void Move() {
        float delta = Time.deltaTime;

        float passedTimeDelta = 0f;
        if (_passedTime > 0f) {
            /* Rather than use a flat catch up rate the
             * extra delta will be based on how much passed time
             * remains. This means the projectile will accelerate
             * faster at the beginning and slower at the end.
             * If a flat rate was used then the projectile
             * would accelerate at a constant rate, then abruptly
             * change to normal move rate. This is similar to using
             * a smooth damp. */

            /* Apply 10% of the step per frame. You can adjust
             * this number to whatever feels good. */
            float step = (_passedTime * 0.1f);
            _passedTime -= step;

            /* If the remaining time is less than half a delta then
             * just append it onto the step. The change won't be noticeable. */
            if (_passedTime <= (delta / 2f)) {
                step += _passedTime;
                _passedTime = 0f;
            }
            passedTimeDelta = step;
        }

        _body.MovePosition(_body.position + _direction * (Speed * (delta + passedTimeDelta)));
    }

    private void Expire() {
        HandleHit(transform.position, true);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (!isActive) {
            return;
        }

        bool isCharacterCollision = collision.gameObject.tag == "Unit";
        bool isWallCollision = collision.gameObject.layer == LayerMask.NameToLayer("Obstacles");

        if (!isCharacterCollision && !isWallCollision) {
            return;
        }

        Vector3 hitLocation = Vector3.zero;

        if (isCharacterCollision) {
            //check if its a character it should hit
            CharacterController hitCharacter = collision.gameObject.GetComponent<CharacterController>();
            NetworkStats hitCharacterStats = hitCharacter.GetComponent<NetworkStats>();

            bool isCaster = hitCharacter == _caster;
            bool isAlly = hitCharacterStats.Faction == _caster.GetComponent<NetworkStats>().Faction;

            bool canHitTarget =
                (isCaster && CanHitCaster) ||
                (isAlly && CanHitAllies) ||
                (!isAlly && CanHitEnemies);

            if (!canHitTarget) {
                return;
            }

            hitLocation = collision.transform.position;

            OnHit?.Invoke(hitLocation, _caster.GetComponent<NetworkStats>(), hitCharacter.GetComponent<NetworkStats>());

            if (_caster.IsOwner) {
                CameraManager.Instance.ShakeCamera(.5f, 0.1f);
            }
        }

        if (isWallCollision) {
            hitLocation = transform.position;
        }

        HandleHit(hitLocation, false);

        CancelInvoke("Expire");
    }

    public void HandleHit(Vector3 hitLocation, bool isExpired = false) {
        CreateHitEffects(hitLocation, isExpired);

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

        if (onHitSFX) {
            AudioClipOptions options = new AudioClipOptions();

            if (isExpired) {
                options.Volume = 0.5f;
                options.Pitch = 0.75f;
            }

            AudioManager.Instance.PlaySound(onHitSFX, transform.position, options);
        }
    }

    public void Reflect(Vector3 reflectPosition, CharacterController _newCaster) {
        Vector2 reflectDirection = _direction * -1;
        Initialise(reflectDirection, 0f, _newCaster);
    }
}