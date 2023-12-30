using System.Collections;
using TMPro;
using UnityEngine;

public class QuestPanel : MonoBehaviour {

    [SerializeField]
    private TMP_Text _title;

    [SerializeField]
    private TMP_Text _objective;

    private void Awake() {
        QuestManager.Instance.OnQuestChanged += HandleScenarioChange;

        HandleScenarioChange(0);
    }

    private void HandleScenarioChange(int newScenarioIndex) {
        Quest newScenario = QuestManager.Instance.GetSelectedQuest();

        SetScenarioText(newScenario);
    }

    private void SetScenarioText(Quest scenario) {
        string newTitle = "No Quest";
        string newObjective = "Select a quest at the Adventurer's Guild!";

        if (scenario != null) {
            newTitle = scenario.Title;
            newObjective = scenario.Objectives[0];
        }

        _title.text = newTitle;
        _objective.text = newObjective;
    }
}