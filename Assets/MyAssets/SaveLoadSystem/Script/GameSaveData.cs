using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSaveData
{
    public string dataName;
    public float gameTime;
    public Dictionary<int, Item> backpack;
    public int moneyCount;
    public string currentScene;
}
