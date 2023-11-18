using FishNet;
using System;
using UnityEngine;

public class PredictedProjectile : Projectile {

    // used to make sure the projectile doesnt last forever
    private const float MAX_LIFE_TIME = 5f;

    private const float MAX_DISTANCE_MODIFIER = -1f;

    private float _passedTime = 0f;

    private Vector2 _startPosition;

    private bool _hasTargetLocation;

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

    private float _targetDistance = 0f;

    private void Awake() {
        _body = GetComponent<Rigidbody2D>();
    }

    public override void Initialise(Vector2 direction, float passedTime, NetworkStats caster, float distance = -1f) {
        base.Initialise(direction, passedTime, caster, distance);

        _passedTime = passedTime;
        _startPosition = transform.position;
        _targetDistance = distance > 0 ? Mathf.Clamp(distance, 0.1f, MaxDistance) : MaxDistance;

        Invoke("Expire", MAX_LIFE_TIME);
    }

    public override void InitialiseTargetLocation(Vector3 targetLocation, float passedTime, NetworkStats caster) {
        _hasTargetLocation = true;
        base.InitialiseTargetLocation(targetLocation, passedTime, caster);
    }

    private void Update() {
        if (!isActive) {
            return;
        }

        if (Vector2.Distance(transform.position, _startPosition) >= _targetDistance) {
            Expire();
        }
    }

    private void FixedUpdate() {
        if (!isActive) {
            return;
        }

        Move();
    }

    protected virtual void Move() {
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

        // TODO this might be better to be addForce but since that function is already multiplied by delta there needs to be some math
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

        bool hasNoCharacterCollision = !isCharacterCollision && !CanHitAllies && !CanHitEnemies;

        if (hasNoCharacterCollision && isCharacterCollision) {
            return;
        }

        Vector3 hitLocation = Vector3.zero;

        if (isCharacterCollision) {
            //check if its a character it should hit
            CharacterController hitCharacter = collision.gameObject.GetComponent<CharacterController>();
            NetworkStats hitCharacterStats = hitCharacter.GetComponent<NetworkStats>();

            bool isCaster = hitCharacter == _caster;
            bool isAlly = hitCharacterStats.Faction == _caster.Faction;

            bool canHitTarget =
                (isCaster && CanHitCaster) ||
                (isAlly && CanHitAllies) ||
                (!isAlly && CanHitEnemies);

            if (!canHitTarget) {
                return;
            }

            hitLocation = collision.transform.position;

            CallOnHit(hitLocation, hitCharacter.GetComponent<NetworkStats>());

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
}