using FishNet.Managing.Scened;
using FishNet;
using FishNet.Object;
using FishNet.Managing.Logging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkSceneLoader : NetworkSingleton<NetworkSceneLoader> {
    private const float SCENE_LOAD_DELAY = 1f;

    private string _sceneToLoad;

    [SerializeField]
    private Animator _fadeAnimator;

    public void LoadScene(string scene) {
        if (base.IsServer) {
            ServerLoadScene(scene);
        } else {
            LoadSceneRpc(scene);
        }
    }

    [ServerRpc]
    private void LoadSceneRpc(string scene) {
        ServerLoadScene(scene);
    }

    private void ServerLoadScene(string scene) {
        _sceneToLoad = scene;
        Invoke(nameof(ServerChangeScene), SCENE_LOAD_DELAY);

        ClientLoadScene();
    }

    [ObserversRpc]
    private void ClientLoadScene() {
        _fadeAnimator.SetTrigger("FadeOut");
    }

    [Server(Logging = LoggingType.Off)]
    private void ServerChangeScene() {
        SceneLoadData data = new SceneLoadData(_sceneToLoad);
        data.ReplaceScenes = ReplaceOption.All;
        InstanceFinder.SceneManager.LoadGlobalScenes(data);
    }
}