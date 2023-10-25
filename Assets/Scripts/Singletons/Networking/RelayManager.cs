using FishNet.Managing;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RelayManager : Singleton<RelayManager> {

    [SerializeField]
    protected NetworkManager _networkManager;

    public virtual Task<string> CreateRelay(string playerId) {
        return null;
    }

    public virtual Task<bool> JoinRelay(string joinCode, string playerId) {
        return null;
    }
}