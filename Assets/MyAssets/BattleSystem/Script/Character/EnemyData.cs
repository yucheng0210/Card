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
    public bool MeleeAttackMode { get; set; }
    public int AttackRange { get; set; }
    public List<ValueTuple<string, int>> CurrentAttackOrderStrs { get; set; }
    public List<ValueTuple<string, int>> AttackOrderStrs { get; set; }
    public int CurrentAttackOrderIndex { get; set; }
    public Dictionary<string, int> PassiveSkills { get; set; }
    public Dictionary<string, int> MaxPassiveSkillsList { get; set; }
    public bool ImageFlip { get; set; }
    public int SpecialAttackCondition { get; set; }
    public List<ValueTuple<string, int>> SpecialAttackOrderStrs { get; set; }
    public Dictionary<string, int> SpecialMechanismList { get; set; }
    public ValueTuple<string, int> SpecialTriggerSkill { get; set; }
    public bool IsMinion { get; set; }
    public EnemyData DeepClone()
    {
        EnemyData clone = (EnemyData)this.MemberwiseClone();
        clone.AttackOrderStrs = new List<ValueTuple<string, int>>(this.AttackOrderStrs);
        clone.PassiveSkills = new Dictionary<string, int>(this.PassiveSkills);
        clone.MaxPassiveSkillsList = new Dictionary<string, int>(this.MaxPassiveSkillsList);
        clone.SpecialAttackOrderStrs = new List<ValueTuple<string, int>>(this.SpecialAttackOrderStrs);
        clone.SpecialMechanismList = new Dictionary<string, int>(this.SpecialMechanismList);
        clone.CurrentAttackOrderStrs = new List<ValueTuple<string, int>>(this.CurrentAttackOrderStrs);
        return clone;
    }
}
