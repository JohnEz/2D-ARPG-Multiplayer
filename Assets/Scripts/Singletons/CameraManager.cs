using UnityEngine;
using System.Collections;
using Cinemachine;

public class CameraManager : Singleton<CameraManager> {

    [SerializeField]
    private CinemachineVirtualCamera playerCamera;

    [SerializeField]
    private Transform playerFollowTarget;

    [SerializeField]
    private GameObject player;

    private Vector3 playerFollowLocation;

    private float _shakeTimer = 0f;

    public void SetFollowTarget(GameObject target) {
        player = target;
    }

    private void Update() {
        ShakeUpdate();

        UpdatePlayerCamera();
    }

    private void UpdatePlayerCamera() {
        if (!player || !playerFollowTarget) {
            return;
        }

        Vector3 mouseWorldPosition = InputHandler.Instance.MouseWorldPosition;
        mouseWorldPosition.z = 0;

        playerFollowLocation = (mouseWorldPosition + player.transform.position) / 2;
        playerFollowLocation = (playerFollowLocation + player.transform.position) / 2;

        playerFollowTarget.position = playerFollowLocation;
    }

    public void ShakeCamera(float intensity, float time) {
        SetShakeIntensity(playerCamera, intensity);
        _shakeTimer = time;
    }

    private void ShakeUpdate() {
        if (_shakeTimer <= 0) {
            return;
        }

        _shakeTimer -= Time.deltaTime;

        if (_shakeTimer <= 0f) {
            SetShakeIntensity(playerCamera, 0f);
        }
    }

    private void SetShakeIntensity(CinemachineVirtualCamera camera, float intensity) {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
    }
}