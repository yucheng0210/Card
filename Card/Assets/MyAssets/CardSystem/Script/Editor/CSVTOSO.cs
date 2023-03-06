using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CsvToSO<T>
{/*
    public static Dictionary<string, List<T>> allData = new Dictionary<string, List<T>>();
    private static string cardListPath = "Assets/MyAssets/CardSystem/Data/Excel/CARDLIST.csv";

    [MenuItem("CsvToSo/ReadExcel/CreateCardSO")]
    public static void CreateCardSO()
    {
        List<T> cardList = new List<T>();
        cardList.Clear();
        string[] allLineData = File.ReadAllLines(cardListPath);
        for (int i = 1; i < allLineData.Length; i++)
        {
            string[] row = allLineData[i].Split(new char[] { ',' });
            CardData_So cardData = ScriptableObject.CreateInstance<CardData_So>();
            cardData.CardID = int.Parse(row[0]);
            cardData.CardName = row[1];
            cardData.CardType = row[2];
            cardData.CardImagePath = row[3];
            cardData.CardCost = int.Parse(row[4]);
            cardData.CardAttribute = row[5];
            cardData.CardEffect = row[6];
            cardData.CardDescription = row[7];
            cardData.cardHeld = int.Parse(row[8]);
            cardList.Add((T)cardData);
            AssetDatabase.CreateAsset(
                cardData,
                $"Assets/MyAssets/CardSystem/Data/SO/{cardData.CardName}.asset"
            );
        }
        allData.Add("CardList", cardList);
        AssetDatabase.SaveAssets();
    }*/
}
