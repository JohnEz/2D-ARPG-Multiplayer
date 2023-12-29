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
public class Scenario {
    public string Title;
    public string Description;
    public List<string> Objectives;
    public SceneAsset scene;

    public int RecommendedLevel = 1;
    public int MaxPlayers = 4;
    public int MinPlayers = 1;
}

public class ScenarioManager : NetworkSingleton<ScenarioManager> {

    [SerializeField]
    private List<Scenario> _scenarios = new List<Scenario>();

    [SyncVar(OnChange = nameof(HandleScenarioChange))]
    private int _selectedScenarioIndex = -1;

    public int SelectedScenarioIndex { get { return _selectedScenarioIndex; } }

    public event Action<int> OnScenarioChanged;

    private bool _isLoadingScenario = false;

    [ServerRpc(RequireOwnership = false)]
    public void SelectScenario(int newScenario) {
        _selectedScenarioIndex = newScenario;
    }

    private void HandleScenarioChange(int prevValue, int newValue, bool asServer) {
        OnScenarioChanged?.Invoke(newValue);
    }

    public Scenario GetSelectedScenario() {
        if (_selectedScenarioIndex == -1) {
            return null;
        }

        if (_selectedScenarioIndex >= _scenarios.Count) {
            return null;
        }

        return _scenarios[_selectedScenarioIndex];
    }

    public bool HasValidScenario() {
        return GetSelectedScenario() != null;
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartScenario() {
        if (_isLoadingScenario) {
            return;
        }

        _isLoadingScenario = true;

        if (!HasValidScenario()) {
            Debug.Log("No scenario selected, go to the Adventurers Guild to select a scenario");
            _isLoadingScenario = false;
            return;
        }

        Scenario scenario = GetSelectedScenario();

        // check if all players are in the area
        // bring up error message

        // load scenario
        SceneLoadData data = new SceneLoadData(scenario.scene.name);
        data.ReplaceScenes = ReplaceOption.All;
        InstanceFinder.SceneManager.LoadGlobalScenes(data);
    }
}