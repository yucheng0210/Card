using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadlineCurseEffect : IEffect
{
    private CardItem cardItem;
    private int reciprocalCount = 2;
    public void ApplyEffect(int value, string target)
    {
        cardItem = BattleManager.Instance.AddCard(5001);
        EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
    }
    private void EventPlayerTurn(params object[] args)
    {
        reciprocalCount--;
        Debug.Log("explosion");
        if (DataManager.Instance.HandCard.Contains(cardItem) && reciprocalCount <= 0)
        {
            string locationID = BattleManager.Instance.CurrentLocationID;
            BattleManager.Instance.TakeDamage(DataManager.Instance.PlayerList[DataManager.Instance.PlayerID], DataManager.Instance.CardList[5001].CardAttack, locationID);
        }
    }
    public Sprite SetIcon()
    {
        return Resources.Load<Sprite>("CardImage/FireBall");
    }
}
