using System.Collections;
using UnityEngine;

[RequireComponent(typeof(InteractableEvent))]
public class StartScenarioArea : MonoBehaviour {
    private InteractableEvent _interactableEvent;

    private void Awake() {
        _interactableEvent = GetComponent<InteractableEvent>();
    }

    private void Start() {
        _interactableEvent.OnInteract += HandleInteracted;
    }

    private void HandleInteracted() {
        if (ScenarioManager.Instance == null) {
            Debug.LogError("No ScenarioManager in scene");
            return;
        }

        if (!ScenarioManager.Instance.HasValidScenario()) {
            Debug.Log("No scenario selected. Go to the Adventurers Guild to select a scenario!");
            return;
        }

        // check to see if all the players are in the area
        if (false) {
            Debug.Log("All players must be at the caravan to leave!");
            return;
        }

        ScenarioManager.Instance.StartScenario();
    }
}