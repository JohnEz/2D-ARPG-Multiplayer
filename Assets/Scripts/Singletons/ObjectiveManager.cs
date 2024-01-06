using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class ScenarioObjective {
    private string _id = System.Guid.NewGuid().ToString();

    public string Id { get { return _id; } }

    public string Description;
    public ScenarioObjectiveType Type;

    // todo we should pull these out into different types OOP
    public NetworkStats KillTarget;
}

public enum ScenarioObjectiveType {
    KILL_TARGET,
}

public class ObjectiveManager : NetworkSingleton<ObjectiveManager> {

    [SerializeField]
    private List<ScenarioObjective> _objectives;

    public List<ScenarioObjective> Objectives { get { return _objectives; } }

    [SyncObject]
    private readonly SyncDictionary<string, bool> _isObjectiveComplete = new();

    public void Start() {
        if (!InstanceFinder.IsServer) {
            return;
        }

        SetupObjectives();
    }

    public void Update() {
        if (!InstanceFinder.IsServer) {
            return;
        }

        if (AreAllPlayersDead()) {
            Defeat();
        }
    }

    private bool AreAllPlayersDead() {
        return GameStateManager.Instance.Players.Count > 0 && GameStateManager.Instance.Players.Find(player => player.CurrentHealth > 0) == null;
    }

    private void SetupObjectives() {
        _isObjectiveComplete.Clear();

        _objectives.ForEach(objective => {
            SetupObjective(objective);
        });

        _isObjectiveComplete.DirtyAll();
    }

    private void SetupObjective(ScenarioObjective objective) {
        _isObjectiveComplete.Add(objective.Id, false);

        switch (objective.Type) {
            case ScenarioObjectiveType.KILL_TARGET:
            SetupKillTarget(objective);
            break;
        }
    }

    private void SetupKillTarget(ScenarioObjective targetScenario) {
        // TODO this is bad because there is no way to remove the function from the action if this singleton is destroyed
        targetScenario.KillTarget.OnHealthDepleted += () => HandleKillTargetDeath(targetScenario);
    }

    private void HandleKillTargetDeath(ScenarioObjective objective) {
        _isObjectiveComplete[objective.Id] = true;

        _isObjectiveComplete.Dirty(objective.Id);

        CheckIfAllObjectivesAreComplete();
    }

    private void CheckIfAllObjectivesAreComplete() {
        if (Objectives.Count == 0) {
            return;
        }

        ScenarioObjective incompleteObjective = Objectives.Find(objective => !_isObjectiveComplete[objective.Id]);

        if (incompleteObjective == null) {
            Victory();
        }
    }

    [Server]
    private void Victory() {
        GameStateManager.Instance.VictoryServer();
    }

    [Server]
    private void Defeat() {
        GameStateManager.Instance.DefeatServer();
    }
}