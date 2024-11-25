using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AddCurseEffect : IEffect
{
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        Dictionary<int, CardData> cardList = DataManager.Instance.CardList;

        // 判斷 value 是否為 -1 以決定遍歷或直接使用 value 作為索引
        if (value == -1)
        {
            for (int i = 0; i < cardList.Count; i++)
            {
                int key = cardList.ElementAt(i).Key;
                CardData cardData = cardList[key];
                if (cardData.CardType == "詛咒")
                {
                    string cardEffect = cardData.CardEffectList[0].Item1;
                    EffectFactory.Instance.CreateEffect(cardEffect).ApplyEffect(value, fromLocation, toLocation);
                }
            }
        }
        else if (cardList.TryGetValue(value, out CardData targetCardData))
        {
            string cardEffect = targetCardData.CardEffectList[0].Item1;
            EffectFactory.Instance.CreateEffect(cardEffect).ApplyEffect(value, fromLocation, toLocation);
        }
    }

    public string SetTitleText()
    {
        return "詛咒";
    }
    public string SetDescriptionText()
    {
        return "將詛咒卡塞入卡組裡。";
    }

}
