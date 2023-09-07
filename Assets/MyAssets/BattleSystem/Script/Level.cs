using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level
{
    public int LevelID { get; set; }
    public string LevelName { get; set; }
    public Dictionary<string, int> EnemyIDList { get; set; }
    public List<ValueTuple<int, int>> RewardIDList { get; set; }
    public string DialogName { get; set; }
    public string LevelType { get; set; }
    public string PlayerStartPos { get; set; }
    public bool LevelPassed { get; set; }
    public List<int> LevelParentList { get; set; }
}
