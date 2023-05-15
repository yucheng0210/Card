using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleManager : Singleton<BattleManager>
{
    private int currentActionPoint;
    private int currentShield;

    public enum BattleType
    {
        None,
        Initial,
        Player,
        Enemy,
        Win,
        Loss
    }

    public void TakeDamage(CharacterData defender, int damage)
    {
        defender.CurrentHealth -= (damage - defender.CurrentShield);
        ((UIBattle)UIManager.Instance.FindUI("UIBattle")).ShowEnemyHealth(
            defender.MaxHealth,
            defender.CurrentHealth
        );
    }

    public void GetShield(int point)
    {
        currentShield += point;
        UIManager.Instance.ShowShieldUI(currentShield);
    }

    public void ConsumeActionPoint(int point)
    {
        if (currentActionPoint >= point)
            currentActionPoint -= point;
        UIManager.Instance.ShowActionPointUI(
            currentActionPoint,
            DataManager.Instance.PlayerList[1].MaxActionPoint
        );
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
                EnemyTurn();
                break;
            case BattleType.Win:
                break;
            case BattleType.Loss:
                break;
        }
    }

    private void PlayerTurn()
    {
        currentActionPoint = DataManager.Instance.PlayerList[0].MaxActionPoint;
        ConsumeActionPoint(0);
    }

    private void EnemyTurn() { }

    public void Shuffle()
    {
        for (int i = 0; i < DataManager.Instance.CardBag.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, DataManager.Instance.CardBag.Count);
            CardData temp = DataManager.Instance.CardBag[randomIndex];
            DataManager.Instance.CardBag[randomIndex] = DataManager.Instance.CardBag[i];
            DataManager.Instance.CardBag[i] = temp;
        }
    }

    public void AddHandCard(int drawCardCount, List<CardItem> cardItems)
    {
        for (int i = 0; i < drawCardCount; i++)
        {
            DataManager.Instance.HandCard.Add(cardItems[i]);
        }
        DataManager.Instance.CardBag.RemoveRange(0, drawCardCount);
    }
}
