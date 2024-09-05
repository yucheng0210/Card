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
    public string EnemyAniPath { get; set; }
    public RectTransform EnemyTrans { get; set; }
    public int AlertDistance { get; set; }
    public int AttackDistance { get; set; }
    public Dictionary<string, int> AttackOrderStrs { get; set; }
    public int CurrentAttackOrder { get; set; }
    public Dictionary<string, int> PassiveSkills { get; set; }
}
