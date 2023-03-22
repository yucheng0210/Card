using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData_SO : CharacterData_SO
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
}
