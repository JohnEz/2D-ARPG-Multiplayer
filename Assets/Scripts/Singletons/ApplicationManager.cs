using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationManager : MonoBehaviour {

    private void Start() {
        Application.targetFrameRate = 144;

#if UNITY_EDITOR
        // hack to set the active scene to a level in debug mode
        int sceneCount = SceneManager.sceneCount;

        for (int i = 0; i < sceneCount; i++) {
            var scene = SceneManager.GetSceneAt(i);

            // dont want to set the main scene to main menu as the sceneloader wont spawn - HACK
            if (!scene.name.Contains("Core") && scene.name != "MainMenu") {
                SceneManager.SetActiveScene(scene);
            }
        }
#else
        if (SceneManager.GetActiveScene().name != "Core") {
            SceneManager.LoadScene("Core");
        }

        SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);

        GameObject networkHudGameObject = GameObject.Find("NetworkManager/NetworkHudCanvas");

        if (networkHudGameObject) {
          networkHudGameObject.SetActive(false);
        }
#endif
    }
}