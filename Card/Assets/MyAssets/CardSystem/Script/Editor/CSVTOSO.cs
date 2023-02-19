using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CSVToSO
{
    //private static string path = "Assets/MyAssets//QuestData/QUESTMANAGER.csv";
    //private static QuestList_SO questList = new QuestList_SO();

  //  [MenuItem("CSVToSO/CreateCardListSO")]
    /*public static void CreateSO()
    {
        string[] allLineData = File.ReadAllLines(path);
        foreach (string s in allLineData)
        {
            string[] row = s.Split(',');
            if (row[0] == "ID")
                continue;
            if (row.Length != 8)
                break;
            Quest_SO quest = ScriptableObject.CreateInstance<Quest_SO>();
            quest.ID = int.Parse(row[0]);
            quest.TheName = row[1];
            quest.NPC = row[2];
            quest.Des = row[3];
            quest.Status = int.Parse(row[4]);
            quest.Rewards = row[5];
            quest.Task = row[6];
            quest.Parent = int.Parse(row[7]);
            questList.QuestList.Add(quest);
            AssetDatabase.CreateAsset(
                quest,
                $"Assets/MyAssets/QuestSystem/QuestData/Quest/{quest.TheName}.asset"
            );
        }

        AssetDatabase.CreateAsset(
            questList,
            "Assets/MyAssets/QuestSystem/QuestData/QuestList/QuestList.asset"
        );
        AssetDatabase.SaveAssets();
    }*/
}

