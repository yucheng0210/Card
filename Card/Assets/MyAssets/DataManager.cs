using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataManager : Singleton<DataManager>
{
    private const string cardListPath = "Assets/MyAssets/Data/CARDLIST.csv";
    public Dictionary<int, CardData> CardList { get; set; }
    public Dictionary<int, CardItem> CardBag { get; set; }
    public Dictionary<int, CardItem> HandCard { get; set; }

    protected override void Awake()
    {
        base.Awake();
        CardList = new Dictionary<int, CardData>();
        LoadData();
    }

    private void LoadData()
    {
        #region 卡牌列表
        string[] lineData = File.ReadAllLines(cardListPath);
        for (int i = 1; i < lineData.Length; i++)
        {
            string[] row = lineData[i].Split(',');
            CardData cardData = new CardData();
            cardData.CardID = int.Parse(row[0]);
            cardData.CardName = row[1];
            cardData.CardType = row[2];
            cardData.CardImagePath = row[3];
            cardData.CardCost = int.Parse(row[4]);
            cardData.CardAttribute = row[5];
            cardData.CardEffect = row[6];
            cardData.CardDescription = row[7];
            CardList.Add(cardData.CardID, cardData);
        }
        #endregion
    }
}
