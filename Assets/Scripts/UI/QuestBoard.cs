using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class QuestBoard : Menu {

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

    private bool _isPlayingQuestAccepted = false;

    public override void Start() {
        base.Start();

        QuestManager.Instance.Quests.ForEach(quest => {
            AddQuest(quest);
        });

        SelectQuest(_questList[0]);

        ResetStamp();
    }

    private void AddQuest(Quest quest) {
        QuestTile tile = Instantiate(_questTilePrefab, _questListTransform);
        tile.SetQuest(quest);

        tile.OnClicked += HandleQuestTileClicked;

        _questList.Add(tile);
    }

    private void HandleQuestTileClicked(QuestTile tile) {
        if (_isPlayingQuestAccepted) {
            return;
        }

        _questList.ForEach(questTile => {
            questTile.Deselect();
        });

        SelectQuest(tile);
    }

    private void SelectQuest(QuestTile tile) {
        if (tile.Quest == _currentlyShownQuest) {
            return;
        }

        _currentlyShownQuest = tile.Quest;

        _questTitle.text = tile.Quest.Title;
        _questDescription.text = tile.Quest.Description;
        _questObjectives.text = tile.Quest.Objectives[0];
        tile.Select();
    }

    public void AcceptQuest() {
        if (_isPlayingQuestAccepted) {
            return;
        }

        _isPlayingQuestAccepted = true;

        _questAcceptedStamp.DOFade(1f, .125f).SetEase(Ease.OutQuad);
        _questAcceptedStamp.transform.DOScale(1f, .125f).SetEase(Ease.OutQuad)
            .OnComplete(HandleStampImpact);
    }

    private void HandleStampImpact() {
        AudioManager.Instance.PlaySound(_questAcceptedSfx);

        Invoke("HandleStampComplete", .8f);
    }

    private void HandleStampComplete() {
        QuestManager.Instance.SelectQuest(_currentlyShownQuest.ID);

        string MyMenuId = GetComponent<Menu>().ID;

        MenuManager.Instance.CloseMenu(MyMenuId);

        ResetStamp();

        _isPlayingQuestAccepted = false;
    }

    private void ResetStamp() {
        _questAcceptedStamp.GetComponent<RectTransform>().localScale = new Vector3(2, 2, 2);

        Color stampColor = Color.white;
        stampColor.a = 0f;
        _questAcceptedStamp.color = stampColor;
    }
}