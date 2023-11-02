using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using System;

public class StatusBarController : MonoBehaviour {

    [SerializeField]
    private TextMeshProUGUI statusText;

    [SerializeField]
    private GameObject _statusPanel;

    [SerializeField]
    private Image statusBar;

    private bool _isDisplayed = true;

    private Buff currentStatus;

    private BuffController myBuffs;

    public event Action OnShowStatus;

    public event Action OnHideStatus;

    // delay variables

    private const float COUNTDOWN_DELAY = .5f;

    private float _delayedTime;

    private float _timeScaler;

    private void OnEnable() {
        ResetStatus();
        HideStatus();

        myBuffs = GetComponentInParent<BuffController>();

        myBuffs.OnBuffsChanged += HandleBuffsChanged;
    }

    private void OnDisable() {
        myBuffs.OnBuffsChanged -= HandleBuffsChanged;
    }

    private void HandleBuffsChanged(List<Buff> buffs) {
        if (buffs.Count == 0) {
            ResetStatus();
            HideStatus();
            return;
        }

        // TODO add a buff priority (eg short stuns should be shown over longer slows)
        Buff buffToDisplay = buffs.OrderByDescending(buff => buff.RemainingTime()).ToList().First();

        SetStatus(buffToDisplay);

        ShowStatus();
    }

    private void SetStatus(Buff newStatus) {
        ResetStatus();
        currentStatus = newStatus;

        float initialRemainingTime = currentStatus.RemainingTime();
        _timeScaler = (initialRemainingTime - COUNTDOWN_DELAY) / initialRemainingTime;

        float fillAmount = currentStatus.RemainingTime() / currentStatus.MaxDuration;
        statusBar.fillAmount = fillAmount;

        statusText.text = currentStatus.Name;
    }

    private void ResetStatus() {
        _timeScaler = 0f;
        _delayedTime = 0f;
        statusBar.fillAmount = 0;
        statusText.text = "";
        currentStatus = null;
    }

    private void HideStatus() {
        if (!_isDisplayed) {
            return;
        }

        _isDisplayed = false;
        _statusPanel.SetActive(false);
        OnHideStatus?.Invoke();
    }

    private void ShowStatus() {
        if (_isDisplayed) {
            return;
        }

        _isDisplayed = true;
        _statusPanel.SetActive(true);
        OnShowStatus?.Invoke();
    }

    private void Update() {
        if (!currentStatus) {
            return;
        }

        if (_delayedTime < COUNTDOWN_DELAY) {
            _delayedTime += Time.deltaTime;
            return;
        }

        float fillAmount = GetFillAmount();
        statusBar.fillAmount = fillAmount;

        if (fillAmount <= 0) {
            // i dont think this is ran because the buff controller removes it first
            //ResetStatus();
            //HideStatus();
        }
    }

    private float GetFillAmount() {
        float remainingTime = currentStatus.RemainingTime();
        float maxDuration = currentStatus.MaxDuration * _timeScaler;

        return remainingTime / maxDuration;
    }
}