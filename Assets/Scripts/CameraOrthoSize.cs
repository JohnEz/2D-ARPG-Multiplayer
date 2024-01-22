using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrthoSize : MonoBehaviour {

    [SerializeField]
    private float baseOrthoSize = 20;

    private void Update() {
        float orthoSize = baseOrthoSize * Screen.height / Screen.width * .5f;

        GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = orthoSize;
    }
}