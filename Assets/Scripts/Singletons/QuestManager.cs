using FishNet.Managing.Scened;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class QuestManager : NetworkSingleton<QuestManager> {

    [SerializeField]
    private List<Quest> _quests = new List<Quest>();

    public List<Quest> Quests { get { return _quests; } }

    [SyncVar(OnChange = nameof(HandleQuestChange))]
    private int _selectedQuestIndex = -1;

    public int SelectedQuestIndex { get { return _selectedQuestIndex; } }

    public event Action<int> OnQuestChanged;

    private bool _isLoadingScene = false;

    private void OnEnable() {
        ConnectionManager.Instance.OnPlayerLoadedScene += HandlePlayerLoadedScene;
    }

    private void OnDisable() {
        if (!ConnectionManager.Instance) {
            return;
        }

        ConnectionManager.Instance.OnPlayerLoadedScene -= HandlePlayerLoadedScene;
    }

    private void HandlePlayerLoadedScene(SessionPlayerData playerData) {
        if (playerData.PersistentPlayer.IsOwner) {
            _isLoadingScene = false;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SelectQuest(string questId) {
        int index = _quests.FindIndex(quest => quest.ID == questId);

        _selectedQuestIndex = index;
    }

    private void HandleQuestChange(int prevValue, int newValue, bool asServer) {
        OnQuestChanged?.Invoke(newValue);
    }

    public Quest GetSelectedQuest() {
        if (_selectedQuestIndex == -1) {
            return null;
        }

        if (_selectedQuestIndex >= _quests.Count) {
            return null;
        }

        return _quests[_selectedQuestIndex];
    }

    public bool HasValidQuest() {
        return GetSelectedQuest() != null;
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartQuest() {
        if (_isLoadingScene) {
            return;
        }

        _isLoadingScene = true;

        if (!HasValidQuest()) {
            Debug.Log("No quest selected, go to the Adventurers Guild to select a quest");
            _isLoadingScene = false;
            return;
        }

        Quest quest = GetSelectedQuest();

        // check if all players are in the area
        // bring up error message

        NetworkSceneLoader.Instance.LoadGameLevel(quest.sceneName);
    }
}