using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
public class UIMap : UIBase
{
    [SerializeField]
    private Transform mapButtonsTrans;
    private Dictionary<int, Sequence> effectList = new();
    private Dictionary<int, Button> mapList = new();
    protected override void Start()
    {
        base.Start();
        StartGame();
    }
    private void Update()
    {
        CanEnterEffect();
    }
    private void StartGame()
    {
        int levelIndex = 0;
        for (int i = 0; i < mapButtonsTrans.childCount; i++)
        {
            int mapID = DataManager.Instance.LevelList.ElementAt(levelIndex).Key;
            if (mapButtonsTrans.GetChild(i).CompareTag("MapSelect"))
            {
                for (int j = 0; j < mapButtonsTrans.GetChild(i).childCount; j++)
                {
                    mapID = DataManager.Instance.LevelList.ElementAt(levelIndex).Key;
                    mapList.Add(mapID, mapButtonsTrans.GetChild(i).GetChild(j).GetComponent<Button>());
                    levelIndex++;
                }
            }
            else
            {
                mapList.Add(mapID, mapButtonsTrans.GetChild(i).GetComponent<Button>());
                levelIndex++;
            }
            mapList[mapID].onClick.AddListener(() => EntryPoint(mapID));
        }
        for (int i = 0; i < mapList.Count; i++)
        {
            int id = mapList.ElementAt(i).Key;
            Sequence scaleSequence = DOTween.Sequence();
            scaleSequence.Append(mapList[id].transform.DOScale(new Vector3(1.25f, 1.25f, 1.25f), 0.5f));
            scaleSequence.Append(mapList[id].transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f));
            scaleSequence.SetLoops(-1);
            effectList.Add(id, scaleSequence);
        }
    }
    private bool CantEnter(int id)
    {
        bool cantEnter = DataManager.Instance.LevelList[id].LevelPassed;
        for (int i = 0; i < DataManager.Instance.LevelList[id].LevelParentList.Count; i++)
        {
            int parentID = DataManager.Instance.LevelList[id].LevelParentList[i];
            if (!DataManager.Instance.LevelList[parentID].LevelPassed)
                cantEnter = true;
        }
        return cantEnter;
    }
    private void CanEnterEffect()
    {
        for (int i = 0; i < effectList.Count; i++)
        {
            int id = effectList.ElementAt(i).Key;
            if (!CantEnter(id))
                effectList[id].Play();
            else
                effectList[id].Pause();
        }
    }
    private void EntryPoint(int id)
    {
        if (CantEnter(id))
            return;
        DataManager.Instance.LevelID = id;
        UIManager.Instance.HideUI("UIMap");
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Explore);
    }
}
