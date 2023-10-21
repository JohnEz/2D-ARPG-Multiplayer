using System.Collections;
using UnityEngine;

public class NetworkDebugUI : MonoBehaviour {
    private NetworkDebugMenu _debugMenu;

    private void Awake() {
        _debugMenu = GetComponentInParent<NetworkDebugMenu>();
    }

    public void OnClickSpawn(string character) {
        _debugMenu.SpawnServer(character);
    }
}