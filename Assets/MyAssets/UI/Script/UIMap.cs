using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
public class UIMap : UIBase
{
    [SerializeField]
    private Transform mapButtonsTrans;
    [SerializeField]
    private List<Sprite> mapTypeList = new List<Sprite>();
    private DG.Tweening.Sequence[][] effectList;
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
        mapList = new Button[MapManager.Instance.MapNodes.Length][];
        effectList = new DG.Tweening.Sequence[MapManager.Instance.MapNodes.Length][];
        for (int i = MapManager.Instance.MapNodes.Length - 1; i >= 0; i--)
        {
            mapList[i] = new Button[MapManager.Instance.MapNodes[i].Length];
            effectList[i] = new DG.Tweening.Sequence[MapManager.Instance.MapNodes[i].Length];
            for (int j = 0; j < MapManager.Instance.MapNodes[i].Length; j++)
            {
                int id = j;
                int count = i;
                int simpleRandomIndex = Random.Range(1001, 1334);
                int normalRandomIndex = Random.Range(1001, 1667);
                int hardRandomIndex = Random.Range(1001, 2000);
                int currentIndex = 0;
                int cumulativeProbability = 0;
                for (int k = 0; k < levelProbabilities.Count; k++)
                {
                    int value = levelProbabilities.ElementAt(k).Value;
                    string key = levelProbabilities.ElementAt(k).Key;
                    int randomValue = Random.Range(0, 100);
                    cumulativeProbability += value;
                    if (cumulativeProbability > randomValue)
                    {
                        switch (key)
                        {
                            case "BATTLE":
                                if (count > 10)
                                    currentIndex = hardRandomIndex;
                                else if (count > 5)
                                    currentIndex = normalRandomIndex;
                                else
                                    currentIndex = simpleRandomIndex;
                                break;
                            case "BOSS":
                                currentIndex = Random.Range(2001, 2003);
                                break;
                            case "RANDOM":
                                currentIndex = 3001;
                                break;
                            case "RECOVER":
                                currentIndex = 4001;
                                break;
                            case "SHOP":
                                currentIndex = 5001;
                                break;
                        }
                        break;
                    }
                }
                if (DataManager.Instance.LevelTypeList[currentIndex].LevelType != "BATTLE" && count == 0)
                {
                    j--;
                    continue;
                }
                Level level = DataManager.Instance.LevelTypeList[currentIndex].Clone();
                level.LevelParentList = new List<int>();
                level.LevelID = i * 5 + j;
                if (MapManager.Instance.MapNodes[i][j].left != null)
                    level.LevelParentList.Add(MapManager.Instance.MapNodes[i][j].left.l.LevelID);
                if (MapManager.Instance.MapNodes[i][j].right != null)
                    level.LevelParentList.Add(MapManager.Instance.MapNodes[i][j].right.l.LevelID);
                MapManager.Instance.MapNodes[i][j].l = level;
                mapList[i][j] = mapButtonsTrans.GetChild(i).GetChild(j).GetComponent<Button>();
                mapList[i][j].onClick.AddListener(() => EntryPoint(count, id));
                DG.Tweening.Sequence scaleSequence = DOTween.Sequence();
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
        if ((MapManager.Instance.LevelCount == 0 && count == 0) || (MapManager.Instance.MapNodes[count][id].l.LevelActive && MapManager.Instance.LevelCount == count))
            return false;
        return true;
    }
    private void CanEnterEffect()
    {
        for (int i = 0; i < MapManager.Instance.MapNodes.Length; i++)
        {
            for (int j = 0; j < MapManager.Instance.MapNodes[i].Length; j++)
            {
                /*  if (i == 1)
                      Debug.Log(i.ToString() + j.ToString() + ":" + !CantEnter(i, j));*/
                if (!CantEnter(i, j))
                {
                    effectList[i][j].Play();
                    Debug.Log("Count:" + i + "  " + "ID:" + j);
                }
                else
                    effectList[i][j].Pause();
            }
        }
        Debug.Log("CurrentLevel: " + MapManager.Instance.LevelCount);
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
