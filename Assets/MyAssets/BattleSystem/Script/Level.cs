using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level
{
    public int LevelID { get; set; }
    public string LevelName { get; set; }
    public Dictionary<int, string> EnemyIDList { get; set; }
    public List<ValueTuple<int, int>> RewardIDList { get; set; }
    public string DialogName { get; set; }
    public Dictionary<int, string> LocationList { get; set; }
}