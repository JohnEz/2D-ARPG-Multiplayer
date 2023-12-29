using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QuestBoard : MonoBehaviour {

    [SerializeField]
    public QuestTile _questTilePrefab;

    [SerializeField]
    public Transform _questListTransform;

    private List<QuestTile> _questList = new List<QuestTile>();

    private void Start() {
        QuestManager.Instance.Quests.ForEach(quest => {
            AddQuest(quest);
        });
    }

    private void AddQuest(Quest quest) {
        QuestTile tile = Instantiate(_questTilePrefab, _questListTransform);
        tile.SetQuest(quest);

        _questList.Add(tile);
    }

    public void SelectQuest() {
    }

    public void AcceptQuest() {
    }
}