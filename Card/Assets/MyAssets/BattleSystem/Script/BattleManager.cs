using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : Singleton<BattleManager>
{
    public enum BattleType
    {
        None,
        Initial,
        Player,
        Enemy,
        Win,
        Loss
    }

    public void ChangeType(BattleType battleType)
    {
        switch (battleType)
        {
            case BattleType.None:
                break;
            case BattleType.Initial:
                break;
            case BattleType.Player:
                break;
            case BattleType.Enemy:
                break;
            case BattleType.Win:
                break;
            case BattleType.Loss:
                break;
        }
    }
}
