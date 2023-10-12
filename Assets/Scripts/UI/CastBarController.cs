using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using DG.Tweening;

public class CastBarController : MonoBehaviour {
    private const float FADE_OUT_TIME = .5f;

    private Image _castBar;

    private float _abilityCastTime;
    private float _passedTime;
    private bool _isCasting = false;

    private float _timeSinceCast;

    private Color _baseColor;

    [SerializeField]
    private Color _successColor;

    [SerializeField]
    private Color _failColor;

    private Sequence fadeSequence;

    // SHOULD BE GLOBAL
    private const float CATCH_UP_PERCENT = 0.08f;

    private void Awake() {
        _castBar = GetComponent<Image>();
        _castBar.fillAmount = 0;
        _baseColor = _castBar.color;
    }

    private void Update() {
        ProgressSlider();
    }

    public void Initialize(CastController castController) {
        castController.OnCastStart.AddListener(HandleCastStart);
        castController.OnCastFail.AddListener(HandleCastCancel);
        castController.OnCastSuccess.AddListener(HandleCastComplete);
    }

    private void ProgressSlider() {
        if (!_isCasting) {
            return;
        }

        float delta = Time.deltaTime;
        float passedTimeDelta = 0f;

        if (_passedTime > 0) {
            float step = (_passedTime * CATCH_UP_PERCENT);
            _passedTime -= step;

            /* If the remaining time is less than half a delta then
             * just append it onto the step. The change won't be noticeable. */
            if (_passedTime <= (delta / 2f)) {
                step += _passedTime;
                _passedTime = 0f;
            }
            passedTimeDelta = step;
        }

        _timeSinceCast += delta + passedTimeDelta;

        float percentComplete = _timeSinceCast / _abilityCastTime;

        _castBar.fillAmount = percentComplete;
    }

    private void HandleCastStart(float castTime, float passedTime) {
        _isCasting = true;
        _abilityCastTime = castTime;
        _passedTime = passedTime;
        _timeSinceCast = 0;
        fadeSequence?.Kill();
        _castBar.color = _baseColor;
    }

    private void HandleCastCancel() {
        EndCast(false);
    }

    private void HandleCastComplete() {
        EndCast(true);
    }

    private void EndCast(bool isComplete) {
        _isCasting = false;

        Color color = isComplete ? _successColor : _failColor;

        _castBar.color = color;

        fadeSequence = DOTween.Sequence();

        fadeSequence.Append(
            _castBar.DOFade(0, FADE_OUT_TIME)
            .SetEase(Ease.InQuad)
            .OnComplete(HideBar)
        );
    }

    private void HideBar() {
        _abilityCastTime = 0;
        _passedTime = 0;
        _castBar.fillAmount = 0;
    }
}