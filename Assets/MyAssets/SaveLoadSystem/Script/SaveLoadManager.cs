using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class SaveLoadManager : Singleton<SaveLoadManager>
{
    private string jsonFolder;
    private List<ISavable> savableList = new List<ISavable>();
    private Dictionary<string, GameSaveData> saveDataDic = new Dictionary<string, GameSaveData>();
    private int currentPathId = 0;

    protected override void Awake()
    {
        base.Awake();
        jsonFolder = Application.dataPath + "/SAVE/";
        DontDestroyOnLoad(this);
    }

    public void AddRegister(ISavable savable)
    {
        savableList.Add(savable);
    }

    public void Save(int id)
    {
        saveDataDic.Clear();
        foreach (var savable in savableList)
        {
            saveDataDic.Add(savable.GetType().Name, savable.GenerateGameData());
        }
        var resultPath = jsonFolder + "data" + (id + 1).ToString() + ".sav";
        var jsonData = JsonConvert.SerializeObject(saveDataDic, Formatting.Indented);
        if (!File.Exists(resultPath))
        {
            Directory.CreateDirectory(jsonFolder);
        }
        File.WriteAllText(resultPath, jsonData);
    }

    public void Load(int id)
    {
        var resultPath = jsonFolder + "data" + (id + 1).ToString() + ".sav";
        if (!File.Exists(resultPath))
        {
            return;
        }
        var stringData = File.ReadAllText(resultPath);
        var jsonData = JsonConvert.DeserializeObject<Dictionary<string, GameSaveData>>(stringData);
        foreach (var savable in savableList)
        {
            /* if (savable.GetType().Name == "SceneController")
                 continue;*/
            savable.RestoreGameData(jsonData[savable.GetType().Name]);
        }
        currentPathId = id;
    }

    public string GetDataName(int id)
    {
        string dataName = "NO DATA";
        var resultPath = jsonFolder + "data" + (id + 1).ToString() + ".sav";
        if (!File.Exists(resultPath))
        {
            return dataName;
        }
        var stringData = File.ReadAllText(resultPath);
        var jsonData = JsonConvert.DeserializeObject<Dictionary<string, GameSaveData>>(stringData);
        return jsonData["SceneController"].dataName;
    }

    public string GetDataTime(int id)
    {
        string dataName = "";
        var resultPath = jsonFolder + "data" + (id + 1).ToString() + ".sav";
        if (!File.Exists(resultPath))
        {
            return dataName;
        }
        var stringData = File.ReadAllText(resultPath);
        var jsonData = JsonConvert.DeserializeObject<Dictionary<string, GameSaveData>>(stringData);
        float gameTime = jsonData["GameManager"].gameTime;
        int hours = (int)(gameTime / 3600);
        int minutes = (int)((gameTime % 3600) / 60);
        int seconds = (int)(gameTime % 60);
        string timeText = "遊玩時間：" + hours.ToString("00") + "：" + minutes.ToString("00") + "：" + seconds.ToString("00");
        return timeText;
    }

    public void AutoSave()
    {
        Save(currentPathId);
    }
}
