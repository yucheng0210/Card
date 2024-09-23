using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeadlineCurseEffect : IEffect
{
    private CardItem cardItem;
    private int reciprocalCount = 3;
    public void ApplyEffect(int value, string target)
    {
        cardItem = BattleManager.Instance.AddCard(5001);
        EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
    }
    private void EventPlayerTurn(params object[] args)
    {
        reciprocalCount--;
        if (reciprocalCount <= 0)
        {
            if (DataManager.Instance.HandCard.Contains(cardItem))
            {
                string locationID = BattleManager.Instance.CurrentLocationID;
                PlayerData playerData = BattleManager.Instance.CurrentPlayerData;
                EnemyData enemyData = BattleManager.Instance.CurrentEnemyList[locationID];
                BattleManager.Instance.TakeDamage(enemyData, playerData, DataManager.Instance.CardList[5001].CardAttack, locationID);
                DataManager.Instance.HandCard.Remove(cardItem);
                DataManager.Instance.RemoveCardBag.Add(cardItem);
                cardItem.gameObject.SetActive(false);
            }
            EventManager.Instance.RemoveEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
        }
    }
    public Sprite SetIcon()
    {
        return Resources.Load<Sprite>("CardImage/FireBall");
    }

    public string SetTitleText()
    {
        throw new System.NotImplementedException();
    }

    public string SetDescriptionText()
    {
        throw new System.NotImplementedException();
    }
}
