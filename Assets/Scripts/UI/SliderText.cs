using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class SliderText : MonoBehaviour {

    [SerializeField]
    private Slider _target;

    private TMP_Text _text;

    private void Start() {
        _text = GetComponent<TMP_Text>();
        _target.onValueChanged.AddListener(HanndleSliderUpdate);

        SetText(_target.value);
    }

    private void HanndleSliderUpdate(float newValue) {
        if (_target == null) {
            return;
        }

        SetText(newValue);
    }

    private void SetText(float value) {
        float percentage = value / _target.maxValue;

        int displayPercentage = (int)(percentage * 100);

        _text.text = $"{displayPercentage}%";
    }
}