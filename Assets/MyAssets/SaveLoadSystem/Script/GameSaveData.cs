using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSaveData
{
    public string DataName { get; set; }
    public float GameTime { get; set; }
    public Dictionary<int, Item> Backpack { get; set; }
    public List<CardData> CardBag { get; set; }
    public List<Potion> PotionBag { get; set; }
    public int MoneyCount { get; set; }
    public string CurrentScene { get; set; }
    public int ChapterCount { get; set; }
    public int LevelCount { get; set; }
    public int LevelID { get; set; }
    public int ISeed { get; set; }
    public bool[][] LevelActiveList { get; set; }
    public List<int> StartSkillList { get; set; }
}
