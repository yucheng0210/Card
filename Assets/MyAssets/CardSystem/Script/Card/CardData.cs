using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
public class CardData
{
    private int cardCost;
    private int cardAttack;
    private int cardShield;
    public int CardID { get; set; }
    public string CardName { get; set; }
    public string CardType { get; set; }
    public string CardImagePath { get; set; }
    public int CardCost { get; set; }
    public string CardAttribute { get; set; }
    public string CardSpecialEffect { get; set; }
    public string CardDescription { get; set; }
    public int CardHeld { get; set; }
    public int CardAttack
    {
        get { return cardAttack; }
        set
        {
            cardAttack = value;
            if (cardAttack < 0)
                cardAttack = 0;
        }
    }
    public int CardShield
    {
        get { return cardShield; }
        set
        {
            cardShield = value;
            if (cardShield < 0)
                cardShield = 0;
        }
    }
    public List<ValueTuple<string, int>> CardEffectList { get; set; }
    public string CardRarity { get; set; }
    public int CardAttackDistance { get; set; }
    public int CardManaCost { get; set; }
    public bool CardRemove { get; set; }
    public int CardBuyPrice { get; set; }
    public bool CardFreeze { get; set; }
    public bool AutoCardRemove { get; set; }
    public bool AutoCardExclusion { get; set; }
    [JsonIgnore]
    public CardItem MyCardItem { get; set; }
    public CardData DeepClone()
    {
        CardData clone = (CardData)this.MemberwiseClone();
        clone.CardEffectList = new List<ValueTuple<string, int>>(this.CardEffectList);
        return clone;
    }
}
