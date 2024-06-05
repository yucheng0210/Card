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
    private List<Sprite> mapTypeList = new List<Sprite>();
    private Sequence[][] effectList;
    private Button[][] mapList;
    private Dictionary<string, int> levelProbabilities = new Dictionary<string, int>
    {
        { "BATTLE", 35 },
        { "BOSS", 5 },
        { "RANDOM", 20 },
        { "RECOVER", 35 },
        { "SHOP",5 }
    };
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
        List<int> removeList = new List<int>();
        for (int i = MapManager.Instance.MapNodes.Length - 1; i >= 0; i--)
        {
            mapList[i] = new Button[MapManager.Instance.MapNodes[i].Length];
            effectList[i] = new Sequence[MapManager.Instance.MapNodes[i].Length];
            for (int j = 0; j < MapManager.Instance.MapNodes[i].Length; j++)
            {
                int id = j;
                int count = i;
                int randomIndex = Random.Range(1001, 1999); ;
                for (int k = 0; k < levelProbabilities.Count; k++)
                {
                    int value = levelProbabilities.ElementAt(k).Value;
                    string key = levelProbabilities.ElementAt(k).Key;
                    int randomValue = Random.Range(0, 100);
                    int cumulativeProbability = 0;
                    if (value > randomValue)
                    {
                        cumulativeProbability += value;
                        switch (key)
                        {
                            case "BATTLE":
                                randomIndex = Random.Range(1001, 1999);
                                break;
                            case "BOSS":
                                randomIndex = Random.Range(2001, 2001);
                                break;
                            case "RANDOM":
                                randomIndex = Random.Range(3001, 3001);
                                break;
                            case "RECOVER":
                                randomIndex = Random.Range(4001, 4001);
                                break;
                            case "SHOP":
                                randomIndex = Random.Range(5001, 5001);
                                break;
                        }
                        break;
                    }
                }
                if (removeList.Contains(randomIndex) || (DataManager.Instance.LevelTypeList[randomIndex].LevelType != "BATTLE" && count == 0))
                {
                    j--;
                    continue;
                }
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
                MapManager.Instance.MapNodes[i][j].transform.localRotation = Quaternion.Euler(0, 0, -90);
                Image mapImage = MapManager.Instance.MapNodes[i][j].GetComponent<Image>();
                string mapType = MapManager.Instance.MapNodes[i][j].l.LevelType;
                SetMapTypeImage(mapImage, mapType);
                Destroy(mapList[i][j].transform.GetChild(0).gameObject);
            }
        }
        CanEnterEffect();

    }
    private void SetMapTypeImage(Image mapImage, string mapType)
    {
        switch (mapType)
        {
            case "BATTLE":
                mapImage.sprite = mapTypeList[0];
                break;
            case "BOSS":
                mapImage.sprite = mapTypeList[1];
                break;
            case "RANDOM":
                mapImage.sprite = mapTypeList[2];
                break;
            case "RECOVER":
                mapImage.sprite = mapTypeList[3];
                break;
            case "SHOP":
                mapImage.sprite = mapTypeList[4];
                break;
                /*case "REMOVECARD":
                    mapImage.sprite = mapTypeList[4];
                    break;*/

        }
    }
    private bool CantEnter(int count, int id)
    {
        bool cantEnter = true;
        if ((MapManager.Instance.LevelCount == 0 && count == 0) || (MapManager.Instance.MapNodes[count][id].l.LevelActive && MapManager.Instance.LevelCount == count))
            cantEnter = false;
        else
            cantEnter = true;
        return cantEnter;
    }
    private void CanEnterEffect()
    {
        for (int i = 0; i < MapManager.Instance.MapNodes.Length; i++)
        {
            for (int j = 0; j < MapManager.Instance.MapNodes[i].Length; j++)
            {
                /*  if (i == 1)
                      Debug.Log(i.ToString() + j.ToString() + ":" + !CantEnter(i, j));*/
                if ((!CantEnter(i, j) && i == MapManager.Instance.LevelCount) || (MapManager.Instance.LevelCount == 0 && i == 0))
                    effectList[i][j].Play();
                else
                    effectList[i][j].Pause();
            }
        }
    }
    private void EntryPoint(int count, int id)
    {
        if (CantEnter(count, id))
            return;
        MapManager.Instance.LevelID = id;
        MapManager.Instance.LevelCount = count;
        UIManager.Instance.HideUI("UIMap");
        if (MapManager.Instance.MapNodes[count][id].left != null)
            MapManager.Instance.MapNodes[count][id].left.l.LevelActive = true;
        if (MapManager.Instance.MapNodes[count][id].right != null)
            MapManager.Instance.MapNodes[count][id].right.l.LevelActive = true;
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Explore);
    }
}
