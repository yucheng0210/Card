using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class UIMap : UIBase
{
    [SerializeField]
    private Dictionary<int, Button> mapList = new();
    protected override void Start()
    {
        base.Start();
        for (int i = 0; i < mapList.Count; i++)
        {
            mapList.ElementAt(i).Value.onClick.AddListener(() => EntryPoint(mapList.ElementAt(i).Key));
        }
    }
    private void EntryPoint(int id)
    {
        DataManager.Instance.LevelID = id;
    }
}
