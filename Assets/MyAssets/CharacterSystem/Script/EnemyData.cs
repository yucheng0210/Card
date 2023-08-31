using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData : CharacterData
{
    public int MinAttack { get; set; }
    public int MaxAttack { get; set; }
    public int CurrentAttack { get; set; }
    public int StepCount { get; set; }
    public string EnemyImagePath { get; set; }
    public RectTransform EnemyTrans { get; set; }
    public string EnemyLocation { get; set; }
}
