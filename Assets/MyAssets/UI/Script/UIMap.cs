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
    [SerializeField]
    private GameObject normalLevelPrefab;
    [SerializeField]
    private GameObject selectLevelPrefab;
    private Sequence[][] effectList;
    private Button[][] mapList;
    protected override void Start()
    {
        base.Start();
        StartGame();
    }
    public override void Show()
    {
        base.Show();
        CanEnterEffect();
    }
    private void StartGame()
    {
        /* int levelIndex = 0;
         for (int i = 0; i < mapButtonsTrans.childCount; i++)
         {
             int mapID = DataManager.Instance.LevelList.ElementAt(levelIndex).Key;
             if (mapButtonsTrans.GetChild(i).CompareTag("MapSelect"))
             {
                 for (int j = 0; j < mapButtonsTrans.GetChild(i).childCount; j++)
                 {
                     mapID = DataManager.Instance.LevelList.ElementAt(levelIndex).Key;
                     int localMapID = mapID;
                     mapList.Add(localMapID, mapButtonsTrans.GetChild(i).GetChild(j).GetComponent<Button>());
                     mapList[localMapID].onClick.AddListener(() => EntryPoint(localMapID));
                     levelIndex++;
                 }
             }
             else
             {
                 int localMapID = mapID;
                 mapList.Add(localMapID, mapButtonsTrans.GetChild(i).GetComponent<Button>());
                 levelIndex++;
                 mapList[localMapID].onClick.AddListener(() => EntryPoint(localMapID));
             }
         }
         for (int i = 0; i < mapList.Count; i++)
         {
             int id = mapList.ElementAt(i).Key;
             Sequence scaleSequence = DOTween.Sequence();
             scaleSequence.Append(mapList[id].transform.DOScale(new Vector3(1.25f, 1.25f, 1.25f), 0.5f));
             scaleSequence.Append(mapList[id].transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f));
             scaleSequence.SetLoops(-1);
             effectList.Add(id, scaleSequence);
         }*/
        mapList = new Button[MapManager.Instance.MapNodes.Length][];
        effectList = new Sequence[MapManager.Instance.MapNodes.Length][];
        for (int i = MapManager.Instance.MapNodes.Length - 1; i >= 0; i--)
        {
            mapList[i] = new Button[MapManager.Instance.MapNodes[i].Length];
            effectList[i] = new Sequence[MapManager.Instance.MapNodes[i].Length];
            for (int j = 0; j < MapManager.Instance.MapNodes[i].Length; j++)
            {
                int id = j;
                int count = i;
                int randomIndex = Random.Range(1001, 1010);
                Level level = DataManager.Instance.LevelTypeList[randomIndex];
                level.LevelParentList = new List<int>();
                level.LevelID = i * 5 + j;
                if (MapManager.Instance.MapNodes[i][j].left != null)
                    level.LevelParentList.Add(MapManager.Instance.MapNodes[i][j].left.l.LevelID);
                if (MapManager.Instance.MapNodes[i][j].right != null)
                    level.LevelParentList.Add(MapManager.Instance.MapNodes[i][j].right.l.LevelID);
                MapManager.Instance.MapNodes[i][j].l = level;
                mapList[i][j] = mapButtonsTrans.GetChild(i).GetChild(j).GetComponent<Button>();
                mapList[i][j].onClick.AddListener(() => EntryPoint(count, id));
                Sequence scaleSequence = DOTween.Sequence();
                scaleSequence.Append(mapList[i][j].transform.DOScale(new Vector3(1.25f, 1.25f, 1.25f), 0.5f));
                scaleSequence.Append(mapList[i][j].transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f));
                scaleSequence.SetLoops(-1);
                effectList[i][j] = scaleSequence;
            }
        }
        CanEnterEffect();
    }

    private bool CantEnter(int count, int id)
    {
        bool cantEnter = true;
        for (int i = 0; i < MapManager.Instance.MapNodes[MapManager.Instance.LevelCount][id].l.LevelParentList.Count; i++)
        {
            if (count == 0)
                cantEnter = false;
            else if (MapManager.Instance.MapNodes[count][i].l.LevelPassed)
                cantEnter = false;
        }
        return cantEnter;
    }
    private void CanEnterEffect()
    {
        for (int i = 0; i < MapManager.Instance.MapNodes.Length; i++)
        {
            for (int j = 0; j < MapManager.Instance.MapNodes[i].Length; j++)
            {
                if ((!CantEnter(i, j) && i == MapManager.Instance.LevelCount) || (MapManager.Instance.LevelCount == 0 && i == 0))
                    effectList[i][j].Play();
                else
                    effectList[i][j].Pause();
            }
        }
    }
    private void EntryPoint(int count, int id)
    {
        Debug.Log(count + "   " + id);
        if (CantEnter(count, id))
            return;
        MapManager.Instance.LevelID = id;
        MapManager.Instance.LevelCount = count;
        UIManager.Instance.HideUI("UIMap");
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Explore);
    }
}
