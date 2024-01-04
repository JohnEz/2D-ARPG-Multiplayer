using FishNet.Object;
using System.Collections;
using UnityEngine;

public class NetworkSingleton<T> : NetworkBehaviour where T : NetworkBehaviour {
    private static T _instance;

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(transform.gameObject);
        }
    }

    public static T Instance {
        get {
            if (_instance == null) {
                _instance = (T)FindObjectOfType(typeof(T));

                if (_instance == null) {
                    //GameObject obj = new GameObject();
                    //_instance = obj.AddComponent<T>();
                    //obj.name = typeof(T).ToString();
                }
            }
            return _instance;
        }
    }
}