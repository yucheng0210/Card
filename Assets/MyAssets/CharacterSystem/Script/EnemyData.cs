using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
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
    public List<ValueTuple<string, int>> AttackOrderStrs { get; set; }
    public int CurrentAttackOrder { get; set; }
    public Dictionary<string, int> PassiveSkills { get; set; }
    public List<string> MaxPassiveSkillsList { get; set; }
    public bool ImageFlip { get; set; }
    public EnemyData DeepClone()
    {
        EnemyData clone = (EnemyData)this.MemberwiseClone();

        // 深拷贝 AttackOrderStrs
        clone.AttackOrderStrs = new List<ValueTuple<string, int>>(this.AttackOrderStrs);

        // 深拷贝 PassiveSkills
        clone.PassiveSkills = new Dictionary<string, int>(this.PassiveSkills);
        clone.MaxPassiveSkillsList = new List<string>(this.MaxPassiveSkillsList);

        return clone;
    }
}
