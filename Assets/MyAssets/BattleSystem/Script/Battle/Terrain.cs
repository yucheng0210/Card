using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrain
{
    public int TerrainID { get; set; }
    public string TerrainName { get; set; }
    public int MaxHealth { get; set; }
    public int CurrentHealth { get; set; }
    public int MinAttack { get; set; }
    public int MaxAttack { get; set; }
    public int AttackDistance { get; set; }
    public string ImagePath { get; set; }
    public Terrain Clone()
    {
        return (Terrain)MemberwiseClone();
    }
}
