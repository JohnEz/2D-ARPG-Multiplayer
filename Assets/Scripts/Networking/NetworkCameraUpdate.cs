using Cinemachine;
using FishNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCameraUpdate : MonoBehaviour {

    private void OnEnable() {
        InstanceFinder.TimeManager.OnTick += UpdateCamera;
    }

    private void OnDisable() {
        if (!InstanceFinder.TimeManager) {
            return;
        }

        InstanceFinder.TimeManager.OnTick -= UpdateCamera;
    }

    private void UpdateCamera() {
        Camera.main.GetComponent<CinemachineBrain>().ManualUpdate();
    }
}