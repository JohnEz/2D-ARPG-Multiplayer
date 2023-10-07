using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour {
    public float DestroyTimer;

    public void Start() {
        //TODO make this work for networkObjects
        Destroy(gameObject, DestroyTimer);
    }
}