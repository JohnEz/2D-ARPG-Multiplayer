using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "2d RPG/New Quest")]
public class Quest : ScriptableObject {
    public string ID;
    public string Title;
    public string Description;
    public List<string> Objectives;

    public string sceneName;

    public int RecommendedLevel = 1;
    public int MaxPlayers = 4;
    public int MinPlayers = 1;
}