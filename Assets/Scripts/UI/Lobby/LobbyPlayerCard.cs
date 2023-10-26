using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct SelectableCharacter {
    public string Name;
    public Color Color;
    public Sprite Sprite;
}

public class LobbyPlayerCard : MonoBehaviour {

    [SerializeField]
    private List<SelectableCharacter> _selectableCharacters;

    private int _selectedCharacterIndex = 0;

    private PersistentPlayer _myPlayer;

    [SerializeField]
    private GameObject _characterSelectCard;

    [SerializeField]
    private GameObject _waitingForPlayer;

    [SerializeField]
    private GameObject _rightArrow;

    [SerializeField]
    private GameObject _leftArrow;

    [SerializeField]
    private Image _characterVisuals;

    [SerializeField]
    private TextMeshProUGUI _characterName;

    [SerializeField]
    private TextMeshProUGUI _playername;

    [SerializeField]
    private GameObject _readyCheck;

    private void Awake() {
        _characterSelectCard.SetActive(false);
        _waitingForPlayer.SetActive(true);
    }

    public void SetPlayer(PersistentPlayer player) {
        if (_myPlayer == player) {
            return;
        }

        PersistentPlayer previousPlayer = _myPlayer;
        _myPlayer = player;

        if (previousPlayer) {
            previousPlayer.OnCharacterChanged -= HandleCharacterChange;
            previousPlayer.OnReadyStateChanged -= HandleReadyChanged;
            previousPlayer.OnUsernameChanged -= HandleNameChanged;
        }

        if (!_myPlayer) {
            HideCharacterSelect();
            return;
        }

        _myPlayer.OnCharacterChanged += HandleCharacterChange;
        _myPlayer.OnReadyStateChanged += HandleReadyChanged;
        _myPlayer.OnUsernameChanged += HandleNameChanged;

        HandleReadyChanged();
        HandleCharacterChange();
        ShowCharacterSelect();

        _playername.text = _myPlayer.Username;
    }

    private void ShowCharacterSelect() {
        _waitingForPlayer.SetActive(false);
        _characterSelectCard.SetActive(true);

        _rightArrow.SetActive(_myPlayer.IsOwner);
        _leftArrow.SetActive(_myPlayer.IsOwner);
    }

    private void HideCharacterSelect() {
        _waitingForPlayer.SetActive(true);
        _characterSelectCard.SetActive(false);
    }

    private void HandleReadyChanged() {
        _readyCheck.SetActive(_myPlayer.IsReady);
    }

    private void HandleNameChanged() {
        _playername.text = _myPlayer.Username;
    }

    private void HandleCharacterChange() {
        _selectedCharacterIndex = _selectableCharacters.FindIndex(character => character.Name == _myPlayer.SelectedCharacter);

        if (_selectedCharacterIndex == -1) {
            _selectedCharacterIndex = 0;

            if (_myPlayer.IsOwner) {
                _myPlayer.SelectedCharacter = _selectableCharacters[_selectedCharacterIndex].Name;
            } else {
                Debug.LogError($"Other clients character was not found: {_myPlayer.SelectedCharacter}");
            }
        }

        SetCharacterVisuals(_selectableCharacters[_selectedCharacterIndex]);
    }

    public void SelectNextCharacter() {
        if (_myPlayer == null) {
            return;
        }

        int newIndex = (_selectedCharacterIndex + 1) % _selectableCharacters.Count;
        _myPlayer.SelectedCharacter = _selectableCharacters[newIndex].Name;
    }

    public void SelectPreviousCharacter() {
        if (_myPlayer == null) {
            return;
        }

        int newIndex = _selectedCharacterIndex - 1;

        if (newIndex < 0) {
            newIndex = _selectableCharacters.Count - 1;
        }

        _myPlayer.SelectedCharacter = _selectableCharacters[newIndex].Name;
    }

    private void SetCharacterVisuals(SelectableCharacter selectableCharacter) {
        _characterVisuals.color = selectableCharacter.Color;
        _characterVisuals.sprite = selectableCharacter.Sprite;
        _characterName.text = selectableCharacter.Name;
    }
}