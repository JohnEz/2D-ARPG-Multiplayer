using System.Collections;
using TMPro;
using UnityEngine;

public class ScenarioPanel : MonoBehaviour {

    [SerializeField]
    private TMP_Text _title;

    [SerializeField]
    private TMP_Text _objective;

    private void Awake() {
        ScenarioManager.Instance.OnScenarioChanged += HandleScenarioChange;

        HandleScenarioChange(0);
    }

    private void HandleScenarioChange(int newScenarioIndex) {
        Scenario newScenario = ScenarioManager.Instance.GetSelectedScenario();

        SetScenarioText(newScenario);
    }

    private void SetScenarioText(Scenario scenario) {
        string newTitle = "";
        string newObjective = "";

        if (scenario != null) {
            newTitle = scenario.Title;
            newObjective = scenario.Objectives[0];
        }

        _title.text = newTitle;
        _objective.text = newObjective;
    }
}