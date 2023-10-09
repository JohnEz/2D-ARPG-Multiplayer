using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class StatusBarController : MonoBehaviour {

    [SerializeField]
    private TextMeshProUGUI statusText;

    [SerializeField]
    private Image statusBar;

    private bool isDisplayed = false;

    private Buff currentStatus;

    private BuffController myBuffs;

    private void OnEnable() {
        ResetStatus();

        myBuffs = GetComponentInParent<BuffController>();

        myBuffs.OnBuffsChanged += HandleBuffsChanged;
    }

    private void OnDisable() {
        myBuffs.OnBuffsChanged -= HandleBuffsChanged;
    }

    private void HandleBuffsChanged(List<Buff> buffs) {
        if (buffs.Count == 0) {
            ResetStatus();
            return;
        }

        // TODO add a buff priority (eg short stuns should be shown over longer slows)
        Buff buffToDisplay = buffs.OrderByDescending(buff => buff.RemainingTime()).ToList().First();

        SetStatus(buffToDisplay);
    }

    private void SetStatus(Buff newStatus) {
        ResetStatus();
        currentStatus = newStatus;

        statusBar.fillAmount = currentStatus.RemainingTime() / currentStatus.Duration;

        statusText.text = currentStatus.Name;
    }

    private void ResetStatus() {
        statusBar.fillAmount = 0;
        statusText.text = "";
        currentStatus = null;
        isDisplayed = false;
    }

    private void Update() {
        if (!currentStatus) {
            return;
        }

        float fillAmount = currentStatus.RemainingTime() / currentStatus.Duration;
        statusBar.fillAmount = fillAmount;

        //Debug.Log(fillAmount);

        if (fillAmount <= 0) {
            ResetStatus();
        }
    }
}