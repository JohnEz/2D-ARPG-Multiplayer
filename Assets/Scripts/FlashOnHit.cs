using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FlashOnHit : MonoBehaviour {

    [SerializeField]
    private Color _flashColor = Color.white;

    [SerializeField]
    private float _flashDuration = 0.25f;

    [SerializeField]
    private AnimationCurve _flashSpeedCurve;

    private List<SpriteRenderer> _spriteRenderers = new();
    private List<Material> _materials = new();

    private Coroutine _flashRoutine;

    private NetworkStats _myStats;

    private void Awake() {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>().ToList();
        _myStats = GetComponent<NetworkStats>();

        Init();
    }

    private void Start() {
        _myStats.OnTakeDamage += Flash;
    }

    private void OnDestroy() {
        if (_myStats) {
            _myStats.OnTakeDamage -= Flash;
        }
    }

    private void Init() {
        _spriteRenderers.ForEach(spriteRenderer => {
            _materials.Add(spriteRenderer.material);
        });
    }

    public void Flash(int damage, bool isShield, NetworkStats source) {
        if (_flashRoutine != null) {
            StopCoroutine(_flashRoutine);
        }

        _flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine() {
        SetFlashColor();

        float currentFlashAmount = 0f;
        float elapsedTime = 0f;

        while (elapsedTime < _flashDuration) {
            elapsedTime += Time.deltaTime;

            currentFlashAmount = Mathf.Lerp(1f, _flashSpeedCurve.Evaluate(elapsedTime), elapsedTime / _flashDuration);

            SetFlashAmount(currentFlashAmount);

            yield return null;
        }
    }

    private void SetFlashColor() {
        _materials.ForEach(material => {
            material.SetColor("_FlashColor", _flashColor);
        });
    }

    private void SetFlashAmount(float amount) {
        _materials.ForEach(material => {
            material.SetFloat("_FlashAmount", amount);
        });
    }
}