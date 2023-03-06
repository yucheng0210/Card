using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : Singleton<BattleManager>
{
    private int currentActionPoint;
    private int currentShield;
    public List<CardData_So> CardList { get; set; }
    public List<PlayerData_SO> PlayerList { get; set; }

    public enum BattleType
    {
        None,
        Initial,
        Player,
        Enemy,
        Win,
        Loss
    }

    public void TakeDamage() { }

    public void GetShield(int point)
    {
        currentShield += point;
        UIManager.Instance.ShowShieldUI(currentShield);
    }

    public void ConsumeActionPoint(int point)
    {
        if (currentActionPoint >= point)
            currentActionPoint -= point;
        UIManager.Instance.ShowActionPointUI(currentActionPoint, PlayerList[0].MaxActionPoint);
    }

    public void ChangeTurn(BattleType battleType)
    {
        switch (battleType)
        {
            case BattleType.None:
                break;
            case BattleType.Initial:
                break;
            case BattleType.Player:
                PlayerTurn();
                break;
            case BattleType.Enemy:
                break;
            case BattleType.Win:
                break;
            case BattleType.Loss:
                break;
        }
    }

    private void PlayerTurn()
    {
        currentActionPoint = PlayerList[0].MaxActionPoint;
        ConsumeActionPoint(0);
    }
}
