using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : Singleton<BattleManager>
{
    [SerializeField]
    private int actionPoint = 3;

    [SerializeField]
    private Text actionPointText;
    public List<CardData_So> CardList { get; set; }

    public enum BattleType
    {
        None,
        Initial,
        Player,
        Enemy,
        Win,
        Loss
    }

    public void ConsumeActionPoint(int point)
    {
        actionPoint -= point;
        UIManager.Instance.ChangeActionPointText(actionPoint);
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
