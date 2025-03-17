using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    public int SkillID { get; set; }
    public string SkillName { get; set; }
    public string SkillDescrption { get; set; }
    public Dictionary<string, int> SkillContent { get; set; }
    public string SkillType { get; set; }
}
