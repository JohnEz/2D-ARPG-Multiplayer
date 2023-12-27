using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class Panel : MonoBehaviour {
    private Vector3 STARTING_SCALE = new Vector3(.7f, .7f, .7f);

    private Vector3 SCALE = Vector3.one;

    public const float IN_ANIMATION_DURATION = .3f;

    public const float OUT_ANIMATION_DURATION = .3f;

    [SerializeField]
    private bool _fadeInOnEnable = true;

    [SerializeField]
    private bool _fadeOutOnDisable = true;

    [SerializeField]
    private bool _scaleInOnEnable = true;

    [SerializeField]
    private bool _scaleOutOnDisable = true;

    [SerializeField]
    private AudioClip _onOpenSFX;

    [SerializeField]
    private AudioClip _onCloseSFX;

    private RectTransform _rectTransform;

    private CanvasGroup _canvasGroup;

    public event Action OnPanelEnabled;

    public event Action OnPanelDisabled;

    private void Awake() {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetPanelEnabled(bool isEnabled, bool withAnimations = true, float delay = 0f) {
        if (isEnabled) {
            EnablePanel(withAnimations, delay);
        } else {
            DisablePanel(withAnimations, delay);
        }
    }

    private void EnablePanel(bool withAnimations, float delay) {
        AudioManager.Instance.PlaySound(_onOpenSFX);

        if (!withAnimations) {
            TurnPanelOn();
            return;
        }

        bool isPlayingAnimation = false;

        if (_fadeInOnEnable) {
            FadeIn(delay);
            isPlayingAnimation = true;
        } else {
            _canvasGroup.alpha = 1;
        }

        if (_scaleInOnEnable) {
            ScaleIn(delay);
            isPlayingAnimation = true;
        } else {
            _rectTransform.localScale = SCALE;
        }

        float timeToWait = isPlayingAnimation ? IN_ANIMATION_DURATION + delay : delay;

        Invoke("TurnPanelOn", timeToWait);
    }

    private void TurnPanelOn() {
        _canvasGroup.alpha = 1;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
        _rectTransform.localScale = SCALE;
        OnPanelEnabled?.Invoke();
    }

    private void TurnPanelOff() {
        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
        _rectTransform.localScale = STARTING_SCALE;
        OnPanelDisabled?.Invoke();
    }

    private void DisablePanel(bool withAnimations, float delay) {
        AudioManager.Instance.PlaySound(_onCloseSFX);

        if (!withAnimations) {
            TurnPanelOff();
            return;
        }

        _canvasGroup.interactable = false;
        bool isPlayingAnimation = false;

        if (_fadeOutOnDisable) {
            FadeOut(delay);
            isPlayingAnimation = true;
        }

        if (_scaleOutOnDisable) {
            ScaleOut(delay);
            isPlayingAnimation = true;
        }

        float timeToWait = isPlayingAnimation ? OUT_ANIMATION_DURATION + delay : delay;
        Invoke("TurnPanelOff", timeToWait);
    }

    private void FadeIn(float delay) {
        _canvasGroup.alpha = 0;
        _canvasGroup.DOFade(1f, IN_ANIMATION_DURATION).SetDelay(delay).SetEase(Ease.OutQuad).SetUpdate(true);
    }

    private void FadeOut(float delay) {
        _canvasGroup.DOFade(0f, OUT_ANIMATION_DURATION).SetDelay(delay).SetEase(Ease.OutQuad).SetUpdate(true);
    }

    private void ScaleIn(float delay) {
        _rectTransform.localScale = STARTING_SCALE;
        _rectTransform.DOScale(SCALE, IN_ANIMATION_DURATION).SetDelay(delay).SetEase(Ease.OutQuad).SetUpdate(true);
    }

    private void ScaleOut(float delay) {
        _rectTransform.DOScale(STARTING_SCALE, OUT_ANIMATION_DURATION).SetDelay(delay).SetEase(Ease.OutQuad).SetUpdate(true);
    }
}