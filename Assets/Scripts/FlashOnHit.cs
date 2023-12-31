using UnityEngine;
using System.Collections;

public class FlashOnHit : MonoBehaviour {

    [SerializeField]
    private SpriteRenderer targetSpriteRenderer;

    private Coroutine flashRoutine;

    private Color originalColor;

    [SerializeField]
    private Color flashColor = Color.white;

    private float flashDuration = 0.1f;

    private NetworkStats _myStats;

    private void Start() {
        originalColor = targetSpriteRenderer.color;

        _myStats = GetComponent<NetworkStats>();

        _myStats.OnTakeDamage += Flash;
    }

    private void OnDestroy() {
        if (_myStats) {
            _myStats.OnTakeDamage -= Flash;
        }
    }

    public void Flash(int damage, bool isShield, NetworkStats source) {
        // If the flashRoutine is not null, then it is currently running.
        if (flashRoutine != null) {
            // In this case, we should stop it first.
            // Multiple FlashRoutines the same time would cause bugs.
            StopCoroutine(flashRoutine);
        }

        flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine() {
        targetSpriteRenderer.color = flashColor;

        yield return new WaitForSeconds(flashDuration);

        targetSpriteRenderer.color = originalColor;

        flashRoutine = null;
    }
}