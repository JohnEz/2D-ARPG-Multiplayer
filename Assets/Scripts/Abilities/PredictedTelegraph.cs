using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.VFX;
using UnityEngine;
using FishNet;

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

    private void Update() {
        // TODO its probably better to have control over the time and following the projectile method
        // currently we just speed up the circle which isnt great
        //_vfx.SetFloat("Delta_Time", Time.deltaTime);
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

        hitCharacter.GetComponent<NetworkStats>().TakeDamage(10, _caster.GetComponent<NetworkStats>().IsOwner);
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