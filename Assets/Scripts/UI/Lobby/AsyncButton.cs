using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AsyncButton : MonoBehaviour {
    private Button _myButton;

    [SerializeField]
    private GameObject _loadingIndicator;

    [SerializeField]
    private GameObject _text;

    private void Awake() {
        _myButton = GetComponent<Button>();
    }

    public void Start() {
        SetInteractable(true);
    }

    public void SetInteractable(bool interactable) {
        _myButton.interactable = interactable;

        if (_loadingIndicator != null) {
            _loadingIndicator.SetActive(!interactable);
        }

        if (_text != null) {
            _text.SetActive(interactable);
        }
    }

    private void OnLoading() {
        SetInteractable(false);
    }

    public void LoadingComplete() {
        SetInteractable(true);
    }
}