using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverMenu : MonoBehaviour {

    public void BackToTown() {
        NetworkSceneLoader.Instance.LoadScene("Town");
    }

    public void Retry() {
    }
}