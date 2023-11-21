using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class AbilityIcon : MonoBehaviour {

    [SerializeField]
    private GameObject _fadeOut;

    [SerializeField]
    private Image _icon;

    [SerializeField]
    private Image _cooldownBar;

    [SerializeField]
    private TextMeshProUGUI _cooldownText;

    [SerializeField]
    private TMP_Text _chargesText;

    private const float HIDE_DURATION = .3f;
    private const float SHOW_DURATION = .3f;

    private Ability _myAbility;

    private bool _isOnCooldown = false;

    private void Awake() {
        _fadeOut.SetActive(false);
        _cooldownText.text = "";
    }

    public void SetAbility(Ability ability) {
        _myAbility = ability;

        _icon.sprite = ability.Icon;

        _isOnCooldown = false;

        _chargesText.text = ability.MaxCharges > 1 ? ability.MaxCharges.ToString() : "";

        _cooldownBar.fillAmount = 0;
    }

    private void Update() {
        if (!_myAbility) {
            return;
        }

        if (_isOnCooldown && !_myAbility.IsOnCooldown()) {
            _isOnCooldown = false;
            ShowIcon();
        } else if (!_isOnCooldown && _myAbility.IsOnCooldown()) {
            _isOnCooldown = true;
            HideIcon();
        }

        if (_isOnCooldown) {
            _cooldownText.text = _myAbility.GetCooldownAsString();
        }

        if (_myAbility.MaxCharges <= 1) {
            return;
        }

        _chargesText.text = _myAbility.CurrentCharges > 0 ? _myAbility.CurrentCharges.ToString() : "";

        if (_myAbility.CurrentCharges > 0 && _myAbility.CurrentCharges < _myAbility.MaxCharges) {
            _cooldownBar.fillAmount = 1 - (_myAbility.GetRemainingCooldown() / _myAbility.Cooldown);
        } else {
            _cooldownBar.fillAmount = 0;
        }
    }

    private void HideIcon() {
        _fadeOut.SetActive(true);
        transform.DOLocalMoveY(-50f, HIDE_DURATION).SetEase(Ease.OutQuart);
    }

    private void ShowIcon() {
        _fadeOut.SetActive(false);
        _cooldownText.text = "";
        transform.DOLocalMoveY(0f, SHOW_DURATION).SetEase(Ease.OutQuart);
    }
}