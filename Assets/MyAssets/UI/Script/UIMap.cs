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
        { "BATTLE", 5 },
        { "BOSS", 5 },
        { "RANDOM", 5 },
        { "RECOVER", 5 },
        { "SHOP",5 },
        {"TREASURE",75}
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
        // 初始化列表
        InitializeLists();

        // 遍歷每一行節點
        for (int i = MapManager.Instance.MapNodes.Length - 1; i >= 0; i--)
        {
            // 初始化每一行的節點數組
            InitializeNodeArrays(i);

            // 遍歷每一行中的每個節點
            for (int j = 0; j < MapManager.Instance.MapNodes[i].Length; j++)
            {
                // 設置節點並檢查是否成功
                if (!SetupNode(i, j))
                {
                    // 如果設置節點失敗，則跳過當前節點
                    j--;
                    continue;
                }

                // 配置按鈕
                ConfigureButton(i, j);

                // 為按鈕設置動畫
                AnimateButton(i, j);

                // 旋轉節點
                RotateNode(i, j);

                // 設置節點的圖像
                SetNodeImage(i, j);

                // 清理節點
                CleanupNode(i, j);
            }
        }

        // 進入效果
        CanEnterEffect();
    }
    #region StartGame
    private void InitializeLists()
    {
        mapList = new Button[MapManager.Instance.MapNodes.Length][];
        effectList = new DG.Tweening.Sequence[MapManager.Instance.MapNodes.Length][];
    }

    private void InitializeNodeArrays(int i)
    {
        mapList[i] = new Button[MapManager.Instance.MapNodes[i].Length];
        effectList[i] = new DG.Tweening.Sequence[MapManager.Instance.MapNodes[i].Length];
    }

    private bool SetupNode(int i, int j)
    {
        int simpleRandomIndex = Random.Range(1001, 1334);
        int normalRandomIndex = Random.Range(1334, 1667);
        int hardRandomIndex = Random.Range(1667, 2000);
        int currentIndex = GetCurrentIndex(i, simpleRandomIndex, normalRandomIndex, hardRandomIndex);

        if (DataManager.Instance.LevelTypeList[currentIndex].LevelType != "BATTLE" && i == 0)
            return false;

        Level level = CreateLevel(i, j, currentIndex);
        MapManager.Instance.MapNodes[i][j].l = level;
        return true;
    }

    private int GetCurrentIndex(int count, int simpleRandomIndex, int normalRandomIndex, int hardRandomIndex)
    {
        int currentIndex = 0;
        int cumulativeProbability = 0;
        if (count == 14)
        {
            currentIndex = 7001;
            return currentIndex;
        }
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
                        /* if (count > 10)
                             currentIndex = hardRandomIndex;
                         else if (count > 5)*/
                        currentIndex = normalRandomIndex;
                        /* else
                             currentIndex = simpleRandomIndex;*/
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
                    case "TREASURE":
                        currentIndex = 6001;
                        break;
                }
                break;
            }
        }
        return currentIndex;
    }

    private Level CreateLevel(int i, int j, int currentIndex)
    {
        Level level = DataManager.Instance.LevelTypeList[currentIndex].Clone();
        level.LevelParentList = new List<int>();
        level.LevelID = i * 5 + j;
        if (MapManager.Instance.MapNodes[i][j].left != null)
            level.LevelParentList.Add(MapManager.Instance.MapNodes[i][j].left.l.LevelID);
        if (MapManager.Instance.MapNodes[i][j].right != null)
            level.LevelParentList.Add(MapManager.Instance.MapNodes[i][j].right.l.LevelID);
        return level;
    }

    private void ConfigureButton(int i, int j)
    {
        mapList[i][j] = mapButtonsTrans.GetChild(i).GetChild(j).GetComponent<Button>();
        mapList[i][j].onClick.AddListener(() => EntryPoint(i, j));
    }

    private void AnimateButton(int i, int j)
    {
        DG.Tweening.Sequence scaleSequence = DOTween.Sequence();
        scaleSequence.Append(mapList[i][j].transform.DOScale(new Vector3(1.25f, 1.25f, 1.25f), 0.5f));
        scaleSequence.Append(mapList[i][j].transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f));
        scaleSequence.SetLoops(-1);
        effectList[i][j] = scaleSequence;
    }

    private void RotateNode(int i, int j)
    {
        MapManager.Instance.MapNodes[i][j].transform.localRotation = Quaternion.Euler(0, 0, -90);
    }

    private void SetNodeImage(int i, int j)
    {
        Image mapImage = MapManager.Instance.MapNodes[i][j].GetComponent<Image>();
        string mapType = MapManager.Instance.MapNodes[i][j].l.LevelType;
        SetMapTypeImage(mapImage, mapType);
    }

    private void CleanupNode(int i, int j)
    {
        Destroy(mapList[i][j].transform.GetChild(0).gameObject);
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
            case "TREASURE":
                mapImage.sprite = mapTypeList[5];
                break;
            case "FINALBOSS":
                mapImage.sprite = mapTypeList[6];
                break;
                /*case "REMOVECARD":
                    mapImage.sprite = mapTypeList[4];
                    break;*/

        }
    }
    #endregion
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
                    //Debug.Log("Count:" + i + "  " + "ID:" + j);
                }
                else
                    effectList[i][j].Pause();
            }
        }
        //Debug.Log("CurrentLevel: " + MapManager.Instance.LevelCount);
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
