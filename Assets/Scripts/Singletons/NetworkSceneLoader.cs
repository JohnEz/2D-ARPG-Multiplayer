using FishNet.Managing.Scened;
using FishNet;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkSceneLoader : NetworkSingleton<NetworkSceneLoader> {
    private const float SCENE_LOAD_DELAY = 1f;

    private string _sceneToLoad;

    [SerializeField]
    private Animator _fadeAnimator;

    [Server]
    public void LoadScene(string scene) {
        _sceneToLoad = scene;

        // start screenfade
        _fadeAnimator.SetTrigger("FadeOut");

        Invoke(nameof(ChangeGlobalScene), SCENE_LOAD_DELAY);
    }

    private void ChangeGlobalScene() {
        SceneLoadData data = new SceneLoadData(_sceneToLoad);
        data.ReplaceScenes = ReplaceOption.All;
        InstanceFinder.SceneManager.LoadGlobalScenes(data);
    }
}