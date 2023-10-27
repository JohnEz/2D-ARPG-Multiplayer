using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingSpinner : MonoBehaviour {

    [SerializeField]
    private RectTransform _rectComponent;

    private float rotateSpeed = 200f;

    private void Update() {
        _rectComponent.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }
}