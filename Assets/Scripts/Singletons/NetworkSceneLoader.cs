using FishNet.Managing.Scened;
using FishNet;
using FishNet.Object;
using FishNet.Managing.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using System.Linq;

public class NetworkSceneLoader : NetworkSingleton<NetworkSceneLoader> {
    private const float SCENE_LOAD_DELAY = 1f;

    private List<string> _scenesToLoad = new();
    private string _activeScene;
    private List<string> _activeScenes = new();

    public event Action OnSceneLoadStart;

    private void Start() {
        int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCount;

        for (int i = 0; i < sceneCount; i++) {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).name;

            if (sceneName != "Core") {
                _activeScenes.Add(sceneName);
            }
        }
    }

    public void LoadScene(string sceneName) {
        List<string> scenes = new List<string>() { sceneName };

        LoadScenes(scenes, sceneName);
    }

    public void LoadGameLevel(string sceneName) {
        LoadScenes(new List<string>() { "LevelCore", sceneName }, sceneName);
    }

    public void LoadScenes(List<string> scenes, string activeScene) {
        if (base.IsServer) {
            ServerLoadScenes(scenes, activeScene);
        } else {
            LoadScenesRpc(scenes, activeScene);
        }
    }

    [ServerRpc]
    private void LoadScenesRpc(List<string> scenes, string activeScene) {
        ServerLoadScenes(scenes, activeScene);
    }

    private void ServerLoadScenes(List<string> scenes, string activeScene) {
        _activeScene = activeScene;
        _scenesToLoad = scenes;

        ClientLoadScene();
        Invoke(nameof(ServerChangeScene), SCENE_LOAD_DELAY);
    }

    [ObserversRpc(RunLocally = true)]
    private void ClientLoadScene() {
        Debug.Log("Client heard to change scene");

        GameObject sceneTransitionGo = GameObject.Find("SceneTransitionCanvas/FadeSceneTransition");

        if (!sceneTransitionGo) {
            Debug.Log("No scene transition found!");
            return;
        }

        Debug.Log("Playing scene transition");
        sceneTransitionGo.GetComponent<Animator>().SetTrigger("FadeOut");
    }

    [Server(Logging = LoggingType.Off)]
    private void ServerChangeScene() {
        List<string> scenesToLoad = _scenesToLoad.Where(sceneToLoad => !_activeScenes.Contains(sceneToLoad)).ToList();
        List<string> scenesToRemove = _activeScenes.Where(activeScene => !_scenesToLoad.Contains(activeScene)).ToList();

        SceneUnloadData unloadData = new SceneUnloadData(scenesToRemove);
        InstanceFinder.SceneManager.UnloadGlobalScenes(unloadData);

        _activeScenes.Clear();

        SceneLoadData loadData = new SceneLoadData(scenesToLoad);
        loadData.ReplaceScenes = ReplaceOption.None;
        loadData.PreferredActiveScene = new SceneLookupData(_activeScene);
        InstanceFinder.SceneManager.LoadGlobalScenes(loadData);

        _scenesToLoad.ForEach(sceneName => _activeScenes.Add(sceneName));
        _scenesToLoad.Clear();

        OnSceneLoadStart?.Invoke();
    }
}