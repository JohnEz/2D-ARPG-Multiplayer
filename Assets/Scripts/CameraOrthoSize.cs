using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrthoSize : MonoBehaviour {

    private void Start() {
        float orthoSize = 30 * Screen.height / Screen.width * .5f;

        GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = orthoSize;
    }
}