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

    private CharacterController _caster;

    public event Action<NetworkStats, NetworkStats> OnHit;

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

        List<NetworkStats> hitCharacters = GetCircleHitTargets(transform.position, _radius);

        foreach (NetworkStats character in hitCharacters) {
            OnHitCharacter(character);
        }

        Destroy(gameObject, 2f);
    }

    private void OnHitCharacter(NetworkStats hitCharacter) {
        if (!hitCharacter) {
            return;
        }

        OnHit?.Invoke(_caster.GetComponent<NetworkStats>(), hitCharacter);
    }

    public static List<NetworkStats> GetCircleHitTargets(Vector3 worldPos, float radius) {
        List<Collider2D> hitTargets = new List<Collider2D>(Physics2D.OverlapCircleAll(new Vector2(worldPos.x, worldPos.y), radius));

        return hitTargets.Where(collider => {
            NetworkStats characterInRange = collider.gameObject.GetComponent<NetworkStats>();

            if (!characterInRange) {
                return false;
            }
            // TODO add faction checking here
            return true;
        }).Select(collider => collider.gameObject.GetComponent<NetworkStats>()).ToList();
    }
}