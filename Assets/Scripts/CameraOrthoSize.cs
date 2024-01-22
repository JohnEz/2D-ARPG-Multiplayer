using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrthoSize : MonoBehaviour {
    //[SerializeField]
    //private float baseOrthoSize = 20;

    [SerializeField]
    private float PIXELS_PER_UNIT = 16;

    [SerializeField]
    private float PPU_SCALE = 2;

    private void Update() {
        //float orthoSize = baseOrthoSize * Screen.height / Screen.width * .5f;
        float orthoSize = ((Screen.height) / (PPU_SCALE * PIXELS_PER_UNIT)) * 0.5f;

        GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = orthoSize;
    }
}