using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Threading.Tasks;

public class UnityLobbyManager : LobbyManager {
    public const float HEART_BEAT_DELAY = 20f;
    public const float LOBBY_UPDATE_POLL_DELAY = 1.1f;

    protected override void CleanUp() {
        base.CleanUp();

        Debug.Log("Cleaning up UnityLobbyManager");

        CancelInvoke("LobbyHeartbeat");
        CancelInvoke("PollForLobbyUpdates");
    }

    public override bool IsHost() {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    protected override Player CreatePlayer() {
        return new Player {
            Data = new Dictionary<string, PlayerDataObject> {
                { KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, PlayerName) },
            }
        };
    }

    public override async void Authenticate(string givenPlayerName) {
        base.Authenticate(givenPlayerName);

        InitializationOptions initializationOptions = new InitializationOptions();
        initializationOptions.SetProfile(PlayerName);

        await UnityServices.InitializeAsync(initializationOptions);

        AuthenticationService.Instance.SignedIn += () => {
            print($"Signed in {AuthenticationService.Instance.PlayerId}");
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        InvokeLoggedIn();
    }

    public override async Task<List<Lobby>> GetLobbies() {
        List<Lobby> activeLobbies = new List<Lobby>();
        try {
            QueryResponse response = await Lobbies.Instance.QueryLobbiesAsync();

            activeLobbies = response.Results;
        } catch (LobbyServiceException e) {
            print(e);
        }

        return activeLobbies;
    }

    public override async void JoinLobbyById(string lobbyId) {
        try {
            JoinLobbyByIdOptions joinLobbyByCodeOptions = new JoinLobbyByIdOptions {
                Player = CreatePlayer()
            };
            Lobby lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, joinLobbyByCodeOptions);

            SetJoinedLobby(lobby);

            if (joinedLobby != null && !IsHost()) {
                await RelayManager.Instance.JoinRelay(joinedLobby.Data[KEY_RELAY_CODE].Value, AuthenticationService.Instance.PlayerId);
            }

            Invoke("PollForLobbyUpdates", LOBBY_UPDATE_POLL_DELAY);
        } catch (LobbyServiceException e) {
            print(e);
        }
    }

    public override async void LeaveLobby() {
        try {
            if (IsHost() && joinedLobby.Players.Count > 1) {
                MigrateLobbyHost();
            }

            // TODO should go in if?
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);

            base.LeaveLobby();
        } catch (LobbyServiceException e) {
            print(e);
        }
    }

    public override async void MigrateLobbyHost() {
        try {
            hostedLobby = await Lobbies.Instance.UpdateLobbyAsync(hostedLobby.Id, new UpdateLobbyOptions {
                HostId = joinedLobby.Players[1].Id
            });

            UpdateLobby(hostedLobby);
        } catch (LobbyServiceException e) {
            print(e);
        }
    }

    public override async Task<bool> CreateLobby(string givenLobbyName) {
        string lobbyName = GenerateLobbyName(givenLobbyName);

        bool createdLobby;
        InvokeLobbyJoined();

        try {
            string joinCode = await RelayManager.Instance.CreateRelay(AuthenticationService.Instance.PlayerId);

            int maxPlayers = 4; // TODO pass this in
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions {
                IsPrivate = false, // TODO pass this in
                Player = CreatePlayer(),
                Data = new Dictionary<string, DataObject> {
                    { KEY_RELAY_CODE, new DataObject(DataObject.VisibilityOptions.Member, joinCode) }
                }
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);

            hostedLobby = lobby;
            SetJoinedLobby(lobby);

            Invoke("LobbyHeartbeat", HEART_BEAT_DELAY);
            Invoke("PollForLobbyUpdates", LOBBY_UPDATE_POLL_DELAY);

            createdLobby = true;

            Debug.Log($"Created lobby {lobbyName}");
        } catch (LobbyServiceException e) {
            print(e);
            createdLobby = false;
        }

        return createdLobby;
    }

    private async void LobbyHeartbeat() {
        if (hostedLobby == null) {
            return;
        }

        await LobbyService.Instance.SendHeartbeatPingAsync(hostedLobby.Id);
        Invoke("LobbyHeartbeat", HEART_BEAT_DELAY);
    }

    // TODO, do i need this if im not going to sit in a lobby and just connect directly with relay?
    private async void PollForLobbyUpdates() {
        if (joinedLobby == null) {
            return;
        }

        Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
        UpdateLobby(lobby);

        if (this) {
            Invoke("PollForLobbyUpdates", LOBBY_UPDATE_POLL_DELAY);
        }

        //TODO check to see if we havent joined the server and there is no a joinCode
    }
}