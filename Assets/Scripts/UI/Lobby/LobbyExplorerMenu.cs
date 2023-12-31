using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyExplorerMenu : MonoBehaviour {
    private List<LobbyCard> lobbyCards = new List<LobbyCard>();

    [SerializeField]
    private Transform lobbyListTransform;

    [SerializeField]
    private GameObject lobbyCardPrefab;

    public void OnEnable() {
        GetComponent<Panel>().OnPanelEnabled += RefreshLobbyList;
    }

    public void OnDisable() {
        GetComponent<Panel>().OnPanelEnabled -= RefreshLobbyList;
    }

    public async void RefreshLobbyList() {
        List<Lobby> lobbies = await LobbyManager.Instance.GetLobbies();

        lobbyCards.ForEach(card => Destroy(card.gameObject));
        lobbyCards = new List<LobbyCard>();

        lobbies.ForEach(lobby => {
            GameObject lobbyCardObject = Instantiate(lobbyCardPrefab, lobbyListTransform);
            LobbyCard card = lobbyCardObject.GetComponent<LobbyCard>();

            card.Initialise(lobby.Id, lobby.Name, lobby.Players.Count, lobby.MaxPlayers);
            lobbyCards.Add(card);
        });
    }
}