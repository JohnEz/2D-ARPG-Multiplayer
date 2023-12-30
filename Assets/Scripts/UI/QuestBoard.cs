using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class QuestBoard : MonoBehaviour {

    [SerializeField]
    public QuestTile _questTilePrefab;

    [SerializeField]
    public Transform _questListTransform;

    private List<QuestTile> _questList = new List<QuestTile>();

    private Quest _currentlyShownQuest = null;

    [SerializeField]
    private TMP_Text _questTitle;

    [SerializeField]
    private TMP_Text _questDescription;

    [SerializeField]
    private TMP_Text _questObjectives;

    private void Start() {
        QuestManager.Instance.Quests.ForEach(quest => {
            AddQuest(quest);
        });

        SelectQuest(QuestManager.Instance.Quests[0]);
    }

    private void AddQuest(Quest quest) {
        QuestTile tile = Instantiate(_questTilePrefab, _questListTransform);
        tile.SetQuest(quest);

        _questList.Add(tile);
    }

    public void SelectQuest(Quest quest) {
        if (quest == _currentlyShownQuest) {
            return;
        }

        _currentlyShownQuest = quest;

        _questTitle.text = quest.Title;
        _questDescription.text = quest.Description;
        _questObjectives.text = quest.Objectives[0];
    }

    public void AcceptQuest() {
        //show some stamp animation then close
        QuestManager.Instance.SelectQuest(_currentlyShownQuest.ID);
    }
}