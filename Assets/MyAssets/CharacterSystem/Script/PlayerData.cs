using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : CharacterData
{
    private int maxActionPoint;
    public int MaxActionPoint
    {
        get { return maxActionPoint; }
        set
        {
            if (maxActionPoint >= 0)
                maxActionPoint = value;
        }
    }
    public int CurrentActionPoint { get; set; }
    public int Mana { get; set; }
    public int DefaultDrawCardCout { get; set; }
}
