using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField]
    private Image _questAcceptedStamp;

    [SerializeField]
    private AudioClip _questAcceptedSfx;

    private void Start() {
        QuestManager.Instance.Quests.ForEach(quest => {
            AddQuest(quest);
        });

        SelectQuest(QuestManager.Instance.Quests[0]);

        ResetStamp();
    }

    private void AddQuest(Quest quest) {
        QuestTile tile = Instantiate(_questTilePrefab, _questListTransform);
        tile.SetQuest(quest);

        tile.OnClicked += HandleQuestTileClicked;

        _questList.Add(tile);
    }

    private void HandleQuestTileClicked(QuestTile tile) {
        _questList.ForEach(questTile => {
            questTile.Deselect();
        });

        SelectQuest(tile.Quest);
        tile.Select();
    }

    private void SelectQuest(Quest quest) {
        if (quest == _currentlyShownQuest) {
            return;
        }

        _currentlyShownQuest = quest;

        _questTitle.text = quest.Title;
        _questDescription.text = quest.Description;
        _questObjectives.text = quest.Objectives[0];
    }

    public void AcceptQuest() {
        DOTween.Sequence()
            .Append(_questAcceptedStamp.DOFade(1f, .125f).SetEase(Ease.OutQuad))
            .Append(_questAcceptedStamp.transform.DOScale(1f, .125f).SetEase(Ease.OutQuad))
            .OnComplete(HandleStampImpact);
    }

    private void HandleStampImpact() {
        AudioManager.Instance.PlaySound(_questAcceptedSfx);

        Invoke("HandleStampComplete", .8f);
    }

    private void HandleStampComplete() {
        QuestManager.Instance.SelectQuest(_currentlyShownQuest.ID);
        MenuManager.Instance.CloseQuestBoard();

        ResetStamp();
    }

    private void ResetStamp() {
        _questAcceptedStamp.GetComponent<RectTransform>().localScale = new Vector3(2, 2, 2);

        Color stampColor = Color.white;
        stampColor.a = 0f;
        _questAcceptedStamp.color = stampColor;
    }
}