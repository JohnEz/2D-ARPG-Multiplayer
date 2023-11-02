using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using DG.Tweening;

public class CastBarController : MonoBehaviour {
    private const float FADE_OUT_TIME = .2f;

    private Image _castBar;

    private float _abilityCastTime;
    private float _passedTime;
    private bool _isCasting = false;
    private float _timeSinceCast;

    private float _channelDuration;
    private float _channelPassedTime;
    private bool _isChanneling = false;
    private float _timeSinceChannel;

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
        ProgressCastSlider();

        ProgressChannelSlider();
    }

    public void Initialize(CastController castController, ChannelController channelController) {
        castController.OnCastStart.AddListener(HandleCastStart);
        castController.OnCastFail.AddListener(HandleCastCancel);
        castController.OnCastSuccess.AddListener(HandleCastComplete);

        channelController.OnChannelStart += HandleChannelStart;
        channelController.OnChannelComplete += HandleCastComplete;
        channelController.OnChannelCancel += HandleCastCancel;
    }

    private void ProgressCastSlider() {
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

    private void ProgressChannelSlider() {
        if (!_isChanneling) {
            return;
        }

        float delta = Time.deltaTime;
        float passedTimeDelta = 0f;

        if (_channelPassedTime > 0) {
            float step = (_channelPassedTime * CATCH_UP_PERCENT);
            _channelPassedTime -= step;

            /* If the remaining time is less than half a delta then
             * just append it onto the step. The change won't be noticeable. */
            if (_channelPassedTime <= (delta / 2f)) {
                step += _channelPassedTime;
                _channelPassedTime = 0f;
            }
            passedTimeDelta = step;
        }

        _timeSinceChannel += delta + passedTimeDelta;

        float percentComplete = _timeSinceChannel / _channelDuration;

        _castBar.fillAmount = 1 - percentComplete;
    }

    private void HandleChannelStart(float duration, float passedTime) {
        _isChanneling = true;
        _channelDuration = duration;
        _channelPassedTime = passedTime;
        _timeSinceChannel = 0f;
        fadeSequence?.Kill();
        _castBar.color = _baseColor; // change to channel colour
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
        if (!_isCasting && !_isChanneling) {
            return;
        }

        _isCasting = false;
        _isChanneling = false;

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