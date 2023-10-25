using FishNet.Managing;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using Unity.Networking.Transport.Relay;
using FishNet.Transporting.UTP;
using static System.Net.WebRequestMethods;

public class UnityRelayManager : RelayManager {

    // TODO move this out of relay?
    protected void SetConnectionPayload(string playerId) {
        //var payload = JsonUtility.ToJson(new ConnectionPayload() {
        //    playerId = playerId,
        //    isDebug = Debug.isDebugBuild
        //});

        //var payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload);

        //NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;
    }

    public override async Task<string> CreateRelay(string playerId) {
        SetConnectionPayload(playerId);

        var utp = (FishyUnityTransport)_networkManager.TransportManager.Transport;

        try {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            utp.SetRelayServerData(new RelayServerData(allocation, "dtls"));

            // Start Server Connection
            _networkManager.ServerManager.StartConnection();
            // Start Client Connection
            _networkManager.ClientManager.StartConnection();

            return joinCode;
        } catch (RelayServiceException e) {
            print(e);
            return null;
        }
    }

    public override async Task<bool> JoinRelay(string joinCode, string playerId) {
        SetConnectionPayload(playerId);

        var utp = (FishyUnityTransport)_networkManager.TransportManager.Transport;

        try {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            utp.SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            // Start Client Connection
            _networkManager.ClientManager.StartConnection();

            return true;
        } catch (RelayServiceException e) {
            print(e);
            return false;
        }
    }
}