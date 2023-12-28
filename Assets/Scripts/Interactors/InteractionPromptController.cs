using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionPromptController : MonoBehaviour {

    [SerializeField]
    private Image _icon;

    [SerializeField]
    private TMP_Text _text;

    public void Show(Sprite icon, string text) {
        _icon.sprite = icon;
        _text.text = text;
    }
}