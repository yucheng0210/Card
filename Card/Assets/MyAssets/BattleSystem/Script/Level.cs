using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level
{
    public int LevelID { get; set; }
    public string LevelName { get; set; }
    public List<ValueTuple<int, int>> EnemyIDList { get; set; }
    public List<ValueTuple<int, int>> RewardIDList { get; set; }
    public string dialogName { get; set; }
}
