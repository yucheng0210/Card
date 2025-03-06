using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowAwayHandCardEffect : IEffect
{
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        List<CardItem> throwAwayList = new List<CardItem>();
        for (int i = 0; i < DataManager.Instance.HandCard.Count; i++)
        {
            throwAwayList.Add(DataManager.Instance.HandCard[i].MyCardItem);
        }
        BattleManager.Instance.ThrowAwayHandCard(throwAwayList, 0.5f);
    }
    public string SetTitleText()
    {
        return "廢棄轉換";
    }
    public string SetDescriptionText()
    {
        return "丟棄所有手牌，每丟棄一張獲得3點魔力。";
    }
}
