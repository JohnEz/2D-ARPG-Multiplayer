using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;

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

        statusBar.fillAmount = currentStatus.RemainingTime() / currentStatus.MaxDuration;

        statusText.text = currentStatus.Name;
    }

    private void ResetStatus() {
        statusBar.fillAmount = 0;
        statusText.text = "";
        currentStatus = null;
    }

    private void HideStatus() {
        if (!_isDisplayed) {
            return;
        }

        Debug.Log("Hiding status");

        _isDisplayed = false;
        _statusPanel.SetActive(false);
    }

    private void ShowStatus() {
        if (_isDisplayed) {
            return;
        }

        Debug.Log("Showing status");

        _isDisplayed = true;
        _statusPanel.SetActive(true);
    }

    private void Update() {
        if (!currentStatus) {
            return;
        }

        float fillAmount = currentStatus.RemainingTime() / currentStatus.MaxDuration;
        statusBar.fillAmount = fillAmount;

        if (fillAmount <= 0) {
            // i dont think this is ran because the buff controller removes it first
            ResetStatus();
            HideStatus();
        }
    }
}