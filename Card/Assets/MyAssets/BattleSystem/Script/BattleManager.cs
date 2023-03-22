using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleManager : Singleton<BattleManager>
{
    private int currentActionPoint;
    private int currentShield;
    public List<CardData_So> CardList { get; set; }
    public List<CardItem> HandCard { get; set; }
    public List<PlayerData_SO> PlayerList { get; set; }
    public List<EnemyData_SO> EnemyList { get; set; }
    public List<CardItem> CardBag { get; set; }

    public enum BattleType
    {
        None,
        Initial,
        Player,
        Enemy,
        Win,
        Loss
    }

    protected override void Awake()
    {
        base.Awake();
        CardBag = new List<CardItem>();
        HandCard = new List<CardItem>();
        PlayerList = new List<PlayerData_SO>();
        EnemyList = new List<EnemyData_SO>();
    }

    public void TakeDamage(CharacterData_SO defender, int damage)
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
private void EnemyTurn()
{
    
}
    public void Shuffle()
    {
        for (int i = CardBag.Count - 1; i >= 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, CardBag.Count);
            CardItem temp = CardBag[randomIndex];
            CardBag[randomIndex] = CardBag[i];
            CardBag[i] = temp;
        }
    }

    public void AddHandCard(int drawCardCount)
    {
        for (int i = 0; i < drawCardCount; i++)
        {
            HandCard.Add(CardBag[i]);
        }
        /*for (int i = 0; i < drawCardCount; i++)
        {
            CardBag.Remove(CardBag[i]);
        }*/
    }
}
