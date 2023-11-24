using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.VFX;
using UnityEngine;
using FishNet;
using System;

public class PredictedTelegraph : MonoBehaviour {
    private VisualEffect _vfx;

    [SerializeField]
    private AudioClip _onCastSFX;

    [SerializeField]
    private AudioClip _onImpactSFX;

    [SerializeField]
    private float impactDelay = 1.5f;

    [SerializeField]
    private float _radius = 2f;

    public bool CanHitAllies = false;

    public bool CanHitCaster = false;

    public bool CanHitEnemies = true;

    private CharacterController _caster;

    public event Action<NetworkStats, List<NetworkStats>> OnHit;

    public event Action<NetworkStats, NetworkStats> OnHitCharacter;

    private void Awake() {
        _vfx = GetComponent<VisualEffect>();
    }

    public void Initialise(float passedTime, CharacterController caster) {
        _caster = caster;

        float lagAdjustedDelay = Mathf.Max(impactDelay - passedTime, 0.1f);

        _vfx.SetFloat("Size", _radius * 2);
        _vfx.SetFloat("Impact_Time", lagAdjustedDelay);

        AudioManager.Instance.PlaySound(_onCastSFX, transform.position);

        Invoke("OnImpact", lagAdjustedDelay);
    }

    private void OnImpact() {
        AudioManager.Instance.PlaySound(_onImpactSFX, transform.position);
        NetworkStats casterStats = _caster.GetComponent<NetworkStats>();

        List<NetworkStats> hitCharacters = GetCircleHitTargets(transform.position, _radius, casterStats, CanHitCaster, CanHitEnemies, CanHitAllies);

        OnHit?.Invoke(casterStats, hitCharacters);

        foreach (NetworkStats character in hitCharacters) {
            HitCharacter(character);
        }

        Destroy(gameObject, 2f);
    }

    private void HitCharacter(NetworkStats hitCharacter) {
        if (!hitCharacter) {
            return;
        }

        OnHitCharacter?.Invoke(_caster.GetComponent<NetworkStats>(), hitCharacter);
    }

    public static List<NetworkStats> GetCircleHitTargets(Vector3 worldPos, float radius, NetworkStats caster, bool canHitCaster, bool canHitEnemies, bool canHitAllies) {
        List<Collider2D> hitTargets = new List<Collider2D>(Physics2D.OverlapCircleAll(new Vector2(worldPos.x, worldPos.y), radius));

        return hitTargets.Where(collider => {
            NetworkStats hitTarget = collider.gameObject.GetComponent<NetworkStats>();

            if (!hitTarget) {
                return false;
            }

            bool isCaster = hitTarget == caster;
            bool isAlly = hitTarget.Faction == caster.Faction && !isCaster;

            bool canHitTarget =
                (isCaster && canHitCaster) ||
                (isAlly && canHitAllies) ||
                (!isAlly && canHitEnemies);

            return canHitTarget;
        }).Select(collider => collider.gameObject.GetComponent<NetworkStats>()).ToList();
    }
}