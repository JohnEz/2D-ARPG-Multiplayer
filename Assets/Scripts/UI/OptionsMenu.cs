using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

public struct Options {
    public float masterVolume;
    public float sfxVolume;
    public float musicVolume;

    public void CopyValuesFrom(Options other) {
        masterVolume = other.masterVolume;
        sfxVolume = other.sfxVolume;
        musicVolume = other.musicVolume;
    }
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
        currentOptions = new Options();
        savedOptions = new Options();
    }

    public override void Start() {
        base.Start();

        masterVolumeSlider.onValueChanged.AddListener(HandleMasterVolumeChange);
        sfxVolumeSlider.onValueChanged.AddListener(HandleSfxVolumeChange);
        musicVolumeSlider.onValueChanged.AddListener(HandleMusicVolumeChange);

        LoadOptions();
    }

    public void LoadOptions() {
        savedOptions.masterVolume = PlayerPrefs.GetFloat("MasterVolume", 50);
        savedOptions.sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 50);
        savedOptions.musicVolume = PlayerPrefs.GetFloat("MusicVolume", 50);

        ResetSliders();
    }

    public void ApplyOptions() {
        SaveOptions();
        Close();
    }

    private void SaveOptions() {
        PlayerPrefs.SetFloat("MasterVolume", currentOptions.masterVolume);
        PlayerPrefs.SetFloat("SfxVolume", currentOptions.sfxVolume);
        PlayerPrefs.SetFloat("MusicVolume", currentOptions.musicVolume);
        PlayerPrefs.Save();

        savedOptions.CopyValuesFrom(currentOptions);
    }

    public void CancelOptions() {
        ResetSliders();
        Close();
    }

    private void ResetSliders() {
        currentOptions.CopyValuesFrom(savedOptions);

        HandleMasterVolumeChange(savedOptions.masterVolume);
        HandleSfxVolumeChange(savedOptions.sfxVolume);
        HandleMusicVolumeChange(savedOptions.musicVolume);

        masterVolumeSlider.value = savedOptions.masterVolume;
        sfxVolumeSlider.value = savedOptions.sfxVolume;
        musicVolumeSlider.value = savedOptions.musicVolume;
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