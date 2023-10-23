using UnityEngine;
using UnityEngine.Audio;

public class AudioClipOptions {
    public float Volume { get; set; } = 1f;
    public float Pitch { get; set; } = 1f;

    public bool RandomPitch = false;

    public float PitchRange = 0.1f;
}

public class AudioManager : Singleton<AudioManager> {

    [SerializeField]
    private AudioMixerGroup musicMixer;

    [SerializeField]
    private AudioMixerGroup sfxMixer;

    public void PlaySound(AudioClip clip, Vector3 worldPosition, AudioClipOptions options = null) {
        if (!clip) {
            return;
        }

        GameObject soundGameObject = CreateSoundGameObject(clip.name, worldPosition);
        CreateAudioSource(clip, soundGameObject, options);
    }

    public void PlaySound(AudioClip clip, Transform parent, AudioClipOptions options = null) {
        if (!clip) {
            return;
        }

        GameObject soundGameObject = CreateSoundGameObject(clip.name, parent);
        CreateAudioSource(clip, soundGameObject, options);
    }

    private GameObject CreateSoundGameObject(string clipName, Vector3 worldPosition) {
        GameObject soundGameObject = new GameObject("Sound " + clipName);
        soundGameObject.transform.position = worldPosition;

        return soundGameObject;
    }

    private GameObject CreateSoundGameObject(string clipName, Transform parent) {
        GameObject soundGameObject = new GameObject("Sound " + clipName);
        soundGameObject.transform.SetParent(parent, false);

        return soundGameObject;
    }

    private void CreateAudioSource(AudioClip clip, GameObject audioObject, AudioClipOptions options) {
        AudioClipOptions audioOptions = options != null ? options : new AudioClipOptions();

        float pitch = audioOptions.RandomPitch ?
            Random.Range(audioOptions.Pitch - audioOptions.PitchRange, audioOptions.Pitch + audioOptions.PitchRange) :
            audioOptions.Pitch;

        AudioSource audioSource = audioObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.maxDistance = 100f;
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.dopplerLevel = 0f;
        audioSource.loop = false;
        audioSource.outputAudioMixerGroup = sfxMixer;
        audioSource.pitch = pitch;
        audioSource.volume = audioOptions.Volume;

        audioSource.Play();

        Destroy(audioObject, audioSource.clip.length);
    }
}