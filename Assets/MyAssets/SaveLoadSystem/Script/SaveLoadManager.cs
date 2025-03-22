using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class SaveLoadManager : Singleton<SaveLoadManager>
{
    private string jsonFolder;
    private List<ISavable> savableList = new List<ISavable>();
    private int currentPathID;
    public int CurrentPathID
    {
        get
        {
            return currentPathID;
        }
        set
        {
            currentPathID = value;
            if (currentPathID > 2)
            {
                currentPathID = 2;
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        jsonFolder = Application.dataPath + "/SAVE/";
    }

    public void AddRegister(ISavable savable)
    {
        savableList.Add(savable);
    }

    public void Save()
    {
        GameSaveData gameSaveData = new GameSaveData();
        for (int i = 0; i < savableList.Count; i++)
        {
            savableList[i].GenerateGameData(gameSaveData);
        }
        var resultPath = jsonFolder + "data" + CurrentPathID.ToString() + ".sav";
        var jsonData = JsonConvert.SerializeObject(gameSaveData, Formatting.Indented);
        if (!File.Exists(resultPath))
        {
            Directory.CreateDirectory(jsonFolder);
        }
        File.WriteAllText(resultPath, jsonData);
    }

    public void Load(int id)
    {
        var resultPath = jsonFolder + "data" + id.ToString() + ".sav";
        if (!File.Exists(resultPath))
        {
            return;
        }
        var stringData = File.ReadAllText(resultPath);
        var jsonData = JsonConvert.DeserializeObject<GameSaveData>(stringData);
        foreach (var savable in savableList)
        {
            /* if (savable.GetType().Name == "SceneController")
                 continue;*/
            savable.RestoreGameData(jsonData);
        }
        CurrentPathID = id;
    }
    public int GetSaveFileCount()
    {
        return Directory.GetFiles(jsonFolder).Length;
    }
    public string GetDataName(int id)
    {
        string dataName = "NO DATA";
        var resultPath = jsonFolder + "data" + id.ToString() + ".sav";
        if (!File.Exists(resultPath))
        {
            return dataName;
        }
        var stringData = File.ReadAllText(resultPath);
        var jsonData = JsonConvert.DeserializeObject<GameSaveData>(stringData);
        return jsonData.dataName;
    }

    public string GetDataTime(int id)
    {
        string dataName = "";
        var resultPath = jsonFolder + "data" + id.ToString() + ".sav";
        if (!File.Exists(resultPath))
        {
            return dataName;
        }
        var stringData = File.ReadAllText(resultPath);
        var jsonData = JsonConvert.DeserializeObject<GameSaveData>(stringData);
        float gameTime = jsonData.gameTime;
        int hours = (int)(gameTime / 3600);
        int minutes = (int)((gameTime % 3600) / 60);
        int seconds = (int)(gameTime % 60);
        string timeText = "遊玩時間：" + hours.ToString("00") + "：" + minutes.ToString("00") + "：" + seconds.ToString("00");
        return timeText;
    }
}
