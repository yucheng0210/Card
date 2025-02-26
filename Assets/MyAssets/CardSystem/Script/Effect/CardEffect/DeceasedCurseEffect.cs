using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeceasedCurseEffect : IEffect
{
    private CardItem cardItem;
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        cardItem = BattleManager.Instance.AddCard(5004);
        EventManager.Instance.AddEventRegister(EventDefinition.eventAttack, EventAttack);
    }
    private void EventAttack(params object[] args)
    {
        List<CardData> handCard = DataManager.Instance.HandCard;
        if (handCard.Contains(cardItem.MyCardData))
        {
            BattleManager.Instance.CurrentPlayerData.MaxHealth--;
            EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
        }
        else
        {
            EventManager.Instance.RemoveEventRegister(EventDefinition.eventAttack, EventAttack);
        }
    }
    public string SetTitleText()
    {
        return "亡者低語";
    }
    public string SetDescriptionText()
    {
        return "每回合抽到此卡扣除一點最大生命。";
    }
}
