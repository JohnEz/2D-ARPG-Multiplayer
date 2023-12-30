using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct Options {
    public float masterVolume;
    public float sfxVolume;
    public float musicVolume;
}

public class OptionsMenu : Menu {

    [SerializeField]
    private Slider masterVolumeSlider;

    [SerializeField]
    private Slider sfxVolumeSlider;

    [SerializeField]
    private Slider musicVolumeSlider;

    private Options currentOptions;

    private Options savedOptions;

    public void Awake() {
        LoadOptions();

        currentOptions = new Options();
        savedOptions = new Options();
    }

    public override void Start() {
        base.Start();

        masterVolumeSlider.onValueChanged.AddListener(HandleMasterVolumeChange);
        sfxVolumeSlider.onValueChanged.AddListener(HandleSfxVolumeChange);
        musicVolumeSlider.onValueChanged.AddListener(HandleMusicVolumeChange);
    }

    public void LoadOptions() {
        savedOptions.masterVolume = PlayerPrefs.GetFloat("MasterVolume", 50);
        savedOptions.sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 50);
        savedOptions.musicVolume = PlayerPrefs.GetFloat("MusicVolume", 50);

        HandleMasterVolumeChange(savedOptions.masterVolume);
        HandleSfxVolumeChange(savedOptions.sfxVolume);
        HandleMusicVolumeChange(savedOptions.musicVolume);

        masterVolumeSlider.SetValueWithoutNotify(savedOptions.masterVolume);
        sfxVolumeSlider.SetValueWithoutNotify(savedOptions.sfxVolume);
        musicVolumeSlider.SetValueWithoutNotify(savedOptions.musicVolume);
    }

    public void SaveOptions() {
        PlayerPrefs.SetFloat("MasterVolume", currentOptions.masterVolume);
        PlayerPrefs.SetFloat("SfxVolume", currentOptions.sfxVolume);
        PlayerPrefs.SetFloat("MusicVolume", currentOptions.musicVolume);
        PlayerPrefs.Save();

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