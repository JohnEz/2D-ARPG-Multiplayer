using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyPlayerCard : MonoBehaviour {
    private PersistentPlayer _myPlayer;

    [SerializeField]
    private GameObject _characterSelectCard;

    [SerializeField]
    private GameObject _waitingForPlayer;

    public void SetPlayer(PersistentPlayer player) {
        _myPlayer = player;

        if (_myPlayer) {
            ShowCharacterSelect();
        } else {
            HideCharacterSelect();
        }
    }

    private void ShowCharacterSelect() {
        _waitingForPlayer.SetActive(false);
        _characterSelectCard.SetActive(true);
    }

    private void HideCharacterSelect() {
        _waitingForPlayer.SetActive(true);
        _characterSelectCard.SetActive(false);
    }
}