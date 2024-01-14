using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverMenu : MonoBehaviour {

    public void BackToTown() {
        NetworkSceneLoader.Instance.LoadGameLevel("Town");
    }

    public void Retry() {
    }
}