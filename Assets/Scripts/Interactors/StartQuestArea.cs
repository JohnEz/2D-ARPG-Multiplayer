using System.Collections;
using UnityEngine;

[RequireComponent(typeof(InteractableEvent))]
public class StartQuestArea : MonoBehaviour {
    private InteractableEvent _interactableEvent;

    private void Awake() {
        _interactableEvent = GetComponent<InteractableEvent>();
    }

    private void Start() {
        _interactableEvent.OnInteract += HandleInteracted;
    }

    private void HandleInteracted() {
        if (QuestManager.Instance == null) {
            Debug.LogError("No ScenarioManager in scene");
            return;
        }

        if (!QuestManager.Instance.HasValidQuest()) {
            Debug.Log("No scenario selected. Go to the Adventurers Guild to select a scenario!");
            return;
        }

        // check to see if all the players are in the area
        if (false) {
            Debug.Log("All players must be at the caravan to leave!");
            return;
        }

        QuestManager.Instance.StartQuest();
    }
}