using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTile : MonoBehaviour {

    [SerializeField]
    private Image _classIcon;

    [SerializeField]
    private TMP_Text _username;

    [SerializeField]
    private TMP_Text _level;

    // TODO move this out
    [SerializeField]
    private Sprite _classIconSprite;

    public void SetPlayer(PersistentPlayer player) {
        _classIcon.sprite = _classIconSprite;
        _username.text = player.Username;
        _level.text = "lvl 1";
    }
}