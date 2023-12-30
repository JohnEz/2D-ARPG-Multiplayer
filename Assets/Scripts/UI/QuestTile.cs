using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuestTile : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler {

    [SerializeField]
    private TMP_Text _text;

    [SerializeField]
    private Image _highlightTexture;

    private Quest _myQuest;

    public Quest Quest { get { return _myQuest; } }

    private bool _isSelected = false;

    [SerializeField]
    private Color _defaultColor = Color.white;

    [SerializeField]
    private Color _hoveredColor = Color.white;

    [SerializeField]
    private Color _selectedColor = Color.white;

    [SerializeField]
    private AudioClip _hoverSfx;

    [SerializeField]
    private AudioClip _clickSfx;

    public event Action<QuestTile> OnClicked;

    private void Awake() {
        SetDefaultColor();
    }

    public void OnPointerClick(PointerEventData eventData) {
        AudioManager.Instance.PlaySound(_clickSfx);

        if (_isSelected) {
            return;
        }

        OnClicked?.Invoke(this);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        AudioManager.Instance.PlaySound(_hoverSfx);

        if (_isSelected) {
            return;
        }

        SetHoveredColor();
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (_isSelected) {
            return;
        }

        SetDefaultColor();
    }

    public void Select() {
        if (_isSelected) {
            return;
        }

        _isSelected = true;
        SetSelectedColor();
    }

    public void Deselect() {
        if (!_isSelected) {
            return;
        }

        _isSelected = false;
        SetDefaultColor();
    }

    public void SetQuest(Quest quest) {
        _myQuest = quest;
        _text.text = quest.Title;
    }

    private void SetHoveredColor() {
        _highlightTexture.color = _hoveredColor;
    }

    private void SetSelectedColor() {
        _highlightTexture.color = _selectedColor;
    }

    private void SetDefaultColor() {
        _highlightTexture.color = _defaultColor;
    }
}