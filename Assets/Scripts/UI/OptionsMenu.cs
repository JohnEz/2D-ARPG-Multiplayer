using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct Options {
    public float masterVolume;
    public float sfxVolume;
    public float musicVolume;
}

public class OptionsMenu : MonoBehaviour {

    [SerializeField]
    private Slider masterVolumeSlider;

    [SerializeField]
    private Slider sfxVolumeSlider;

    [SerializeField]
    private Slider musicVolumeSlider;

    private Options currentOptions;

    private Options savedOptions;

    private void Awake() {
        LoadOptions();

        currentOptions = new Options();
        savedOptions = new Options();
    }

    private void Start() {
        masterVolumeSlider.onValueChanged.AddListener(HandleMasterVolumeChange);
        sfxVolumeSlider.onValueChanged.AddListener(HandleSfxVolumeChange);
        musicVolumeSlider.onValueChanged.AddListener(HandleMusicVolumeChange);
    }

    public void LoadOptions() {
    }

    public void SaveOptions() {
        savedOptions = currentOptions;
    }

    public void CancelOptions() {
        currentOptions = savedOptions;
    }

    private void HandleMasterVolumeChange(float value) {
        AudioManager.Instance.SetMasterVolume(value);
        currentOptions.masterVolume = value;
    }

    private void HandleSfxVolumeChange(float value) {
        AudioManager.Instance.SetSfxVolume(value);
        currentOptions.sfxVolume = value;
    }

    private void HandleMusicVolumeChange(float value) {
        AudioManager.Instance.SetMusicVolume(value);
        currentOptions.musicVolume = value;
    }
}