using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CastBarController : MonoBehaviour {
    private Image castBar;

    private float _abilityCastTime;
    private float _passedTime;
    private bool _isCasting = false;

    private float _timeSinceCast;

    // SHOULD BE GLOBAL
    private const float CATCH_UP_PERCENT = 0.08F;

    private void Awake() {
        castBar = GetComponent<Image>();
        castBar.fillAmount = 0;
    }

    private void Update() {
        ProgressSlider();
    }

    public void Initialize(CastController castController) {
        castController.OnCastStart.AddListener(HandleCastStart);
        castController.OnCastFail.AddListener(HandleCastCancel);
        castController.OnCastSuccess.AddListener(HandleCastCancel);
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

        castBar.fillAmount = percentComplete;

        if (percentComplete >= 1) {
            HideBar();
        }
    }

    private void HandleCastStart(float castTime, float passedTime) {
        _isCasting = true;
        _abilityCastTime = castTime;
        _passedTime = passedTime;
        _timeSinceCast = 0;
    }

    private void HandleCastCancel() {
        HideBar();
    }

    private void HideBar() {
        _isCasting = false;
        _abilityCastTime = 0;
        _passedTime = 0;
        castBar.fillAmount = 0;
    }
}