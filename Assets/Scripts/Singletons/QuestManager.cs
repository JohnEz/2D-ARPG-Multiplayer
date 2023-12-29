using FishNet.Managing.Scened;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class Quest {
    public string Title;
    public string Description;
    public List<string> Objectives;
    public SceneAsset scene;

    public int RecommendedLevel = 1;
    public int MaxPlayers = 4;
    public int MinPlayers = 1;
}

public class QuestManager : NetworkSingleton<QuestManager> {

    [SerializeField]
    private List<Quest> _quests = new List<Quest>();

    public List<Quest> Quests { get { return _quests; } }

    [SyncVar(OnChange = nameof(HandleQuestChange))]
    private int _selectedQuestIndex = -1;

    public int SelectedQuestIndex { get { return _selectedQuestIndex; } }

    public event Action<int> OnQuestChanged;

    private bool _isLoadingScene = false;

    [ServerRpc(RequireOwnership = false)]
    public void SelectQuest(int newQuest) {
        _selectedQuestIndex = newQuest;
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

        // load scene
        SceneLoadData data = new SceneLoadData(quest.scene.name);
        data.ReplaceScenes = ReplaceOption.All;
        InstanceFinder.SceneManager.LoadGlobalScenes(data);
    }
}