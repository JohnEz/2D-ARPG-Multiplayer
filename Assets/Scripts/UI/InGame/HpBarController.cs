using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBarController : MonoBehaviour {
    public Image hpBar;
    public Image shieldBar;
    public Image damageBar;
    public Image healableHealthBar;

    private const float DAMAGE_BAR_SHRINK_TIMER_MAX = 0.5f;

    private float damageBarShrinkTimer;
    private float targetHPPercent = 1;
    private float targetShieldPercent = 0;
    private float targetHealablePercent = 1;

    private NetworkStats _myStats;

    public void Initialize(NetworkStats stats) {
        _myStats = stats;
        _myStats.OnHealthChanged += SetHp;

        SetHp();
    }

    public void OnDisable() {
        if (_myStats != null) {
            _myStats.OnHealthChanged -= SetHp;
        }
    }

    public void OnEnable() {
        if (_myStats != null) {
            _myStats.OnHealthChanged += SetHp;
        }
    }

    // Update is called once per frame
    private void Update() {
        //UpdateBarToValue(shieldBar, targetShieldPercent);
        //UpdateBarToValue(hpBar, targetHPPercent);

        if (damageBarShrinkTimer < 0) {
            UpdateBarToValue(damageBar, hpBar.fillAmount);
        } else {
            damageBarShrinkTimer -= Time.deltaTime;
        }
    }

    public static void UpdateBarToValue(Image bar, float targetAmount) {
        if (targetAmount == bar.fillAmount) {
            return;
        }
        bar.fillAmount = Mathf.Lerp(bar.fillAmount, targetAmount, 5f * Time.deltaTime);

        float distance = Mathf.Abs(targetAmount - bar.fillAmount);
        if (distance < 0.005f) {
            bar.fillAmount = targetAmount;
        }
    }

    public void SetHp() {
        float health = _myStats.CurrentHealth;
        float shield = _myStats.Shield.CurrentValue;
        float maxHealth = _myStats.MaxHealth.CurrentValue;
        float healableHealth = _myStats.RemainingMaxHealth;

        targetHPPercent = health / (maxHealth + shield);
        targetHealablePercent = healableHealth / (maxHealth + shield);
        targetShieldPercent = (health + shield) / (maxHealth + shield);

        hpBar.fillAmount = targetHPPercent;
        shieldBar.fillAmount = targetShieldPercent;
        healableHealthBar.fillAmount = targetHealablePercent;

        damageBarShrinkTimer = DAMAGE_BAR_SHRINK_TIMER_MAX;
    }

    public void SetHPColor(Color color) {
        hpBar.color = color;
    }
}