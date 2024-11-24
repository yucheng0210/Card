using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapData
{
    public int TrapID { get; set; }
    public string TrapName { get; set; }
    public int MaxHealth { get; set; }
    public int CurrentHealth { get; set; }
    public int BaseAttack { get; set; }
    public int CurrentAttack { get; set; }
    public Dictionary<string, int> TriggerSkillList { get; set; }
    public string TrapImagePath { get; set; }
    public Transform TrapTrans { get; set; }
    public TrapData DeepClone()
    {
        TrapData clone = (TrapData)this.MemberwiseClone();
        clone.TriggerSkillList = new Dictionary<string, int>(this.TriggerSkillList);
        return clone;
    }
}
