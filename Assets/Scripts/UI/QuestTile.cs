using System.Collections;
using TMPro;
using UnityEngine;

public class QuestTile : MonoBehaviour {

    [SerializeField]
    private TMP_Text _text;

    public void SetQuest(Quest quest) {
        _text.text = quest.Title;
    }

    public void Highlight() {
    }

    public void Unhighlight() {
    }
}