using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class UIMap : UIBase
{
    private Dictionary<int, Button> mapList = new();
    [SerializeField]
    private Transform mapButtonsTrans;
    protected override void Start()
    {
        base.Start();
        StartGame();
    }
    private void StartGame()
    {
        for (int i = 0; i < mapButtonsTrans.childCount; i++)
        {
            int mapID = DataManager.Instance.LevelList.ElementAt(i).Key;
            mapList.Add(mapID, mapButtonsTrans.GetChild(i).GetComponent<Button>());
            mapList[mapID].onClick.AddListener(() => EntryPoint(mapID));
        }
    }

    private void EntryPoint(int id)
    {
        Debug.Log(id);
        DataManager.Instance.LevelID = id;
        UIManager.Instance.HideUI("UIMap");
    }
}
