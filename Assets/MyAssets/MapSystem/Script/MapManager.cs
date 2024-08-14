using System;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{
    [SerializeField] int maxLevel = 0;
    [SerializeField] int maxCount = 0;
    public int iSeed;
    public System.Random random;
    MapNode[][] mapNodes;
    [SerializeField] int leftPadding;
    [SerializeField] int paddingX;
    [SerializeField] int paddingY;
    [SerializeField] EveryLevel[] levels;
    [SerializeField]
    private Transform mapTrans;
    [SerializeField]
    private Material lineMaterial;
    public MapNode[][] MapNodes { get { return mapNodes; } }
    public int LevelCount { get; set; }
    public int LevelID { get; set; }
    protected override void Awake()
    {
        base.Awake();
        Init();
    }

    void Init()
    {
        iSeed = UnityEngine.Random.Range(1, 99999);
        random = new System.Random(iSeed);
        mapNodes = new MapNode[maxLevel][];
        int count = 0;
        for (int i = 0; i < mapNodes.Length; i++)
        {
            mapNodes[i] = levels[i].nodes;
            for (int j = 0; j < mapNodes[0].Length; j++)
            {
                mapNodes[i][j].level = i;
                mapNodes[i][j].value = count++;
            }
        }
        CreateMap();
        // LogTrue();
        // SerTree();
        ShowRooms();
        SetLevelsPosition();
        ShowLine();
    }
    void CreateMap()
    {
        CreateTheFirstLevel();
        for (int i = 0; i < maxLevel - 1; i++)
        {
            ProcessThisLevel(i);
        }
    }
    void ProcessThisLevel(int currentLevel)
    {
        // v0.0
        // for (int i = 0; i < maxCount; i++)
        // {
        //     if (mapNodes[currentLevel][i].isUsed)
        //     {
        //         MapNode nextLevelNode = mapNodes[currentLevel + 1][random.Next(maxCount)];
        //         mapNodes[currentLevel][i].left = nextLevelNode;
        //         nextLevelNode.isUsed = true;
        //         nextLevelNode = mapNodes[currentLevel + 1][random.Next(maxCount)];
        //         mapNodes[currentLevel][i].right = nextLevelNode == mapNodes[currentLevel][i].left ? null : nextLevelNode;
        //         nextLevelNode.isUsed = true;
        //     }
        //     // else
        //     // {
        //     //     mapNodes[currentLevel][i] = null;
        //     // }
        // }


        //v1.0
        //选定这一层的一个点
        //确定这个点是被使用的，即被连接的，如果不是，置空
        //从这个点对应的下层的同一位置开始选择如{x，y}对应的下层{x+1，y}
        //普遍情况下，可以选择的点为{x+1，y-1}，{x+1，y}，{x+1，y+1}
        //如果点{x，y}的y为0，则不能选择{x+1，y-1}，同理y为maxCount，不能选择{x+1，y+1}
        //如果{x+1，y}已经被{x，y-1}连接过，{x，y}只可以选择{x+1，y}，{x+1，y+1}进行连接

        //v2.0
        //选定这一层的一个点
        //确定这个点是被使用的，即被连接的，如果不是，不做任何处理
        //对于连接一下层的点，使用了新的方式
        //首先确定声明一个变量nextLevelLeftPoint，作为下一层的可连接点的左边界
        //和1.0版本一样，查询{x+1，y}的点是不是被连接过，如果被连接过，或者是nextLevelLeftPoint在{x+1，y}的右边（包括他自己），{x+1，y-1}这个点不能连接，也就是左边的点不能连接，即设置canLeft=false
        //如果x是这一行最后一个点，或者nextLevelLeftPoint是下一行的最后一个点，自然就不能连接{x+1，y+1}，也就是右边的点不能连接，所以canRight=false
        //同样的，如果x是这一行的第一个点，左边也是不能连接的，canLeft=false；
        //然后判断可以选择的点有几个，直接产生两个随机数，然后连接，再nextLevelLeftPoint += secondNum，也就是让nextLevelLeftPoint移动到所有被连接的点的最右边，即可连接点的左边界
        if (currentLevel == maxLevel - 2)
        {
            ProcessLastLevel(currentLevel);
            return;
        }
        MapNode beUsedNode;
        bool canLeft = true;
        bool canRight = true;
        int firstNum = 0;
        int secondNum = 0;
        int nextLevelLeftPoint = 0;
        for (int i = 0; i < maxCount; i++)
        {
            if (mapNodes[currentLevel][i].isUsed)
            {
                if (mapNodes[currentLevel + 1][i].isUsed || nextLevelLeftPoint >= i)
                {
                    canLeft = false;
                }
                if (i >= maxCount - 1 || nextLevelLeftPoint >= maxCount - 1)
                {
                    canRight = false;
                }
                if (i == 0)
                {
                    canLeft = false;
                }
                if (canLeft && canRight)
                {
                    firstNum = random.Next(3);
                    secondNum = random.Next(3);
                }
                else if (canRight || canLeft)
                {
                    firstNum = random.Next(2);
                    secondNum = random.Next(2);
                }
                //确保firstNum <= secondNum
                if (firstNum > secondNum)
                {
                    firstNum = firstNum ^ secondNum;
                    secondNum = firstNum ^ secondNum;
                    firstNum = firstNum ^ secondNum;
                }
                beUsedNode = mapNodes[currentLevel + 1][nextLevelLeftPoint + firstNum];
                beUsedNode.isUsed = true;
                mapNodes[currentLevel][i].left = beUsedNode;
                beUsedNode = mapNodes[currentLevel + 1][nextLevelLeftPoint + secondNum];
                beUsedNode.isUsed = true;
                mapNodes[currentLevel][i].right = beUsedNode;
                nextLevelLeftPoint += secondNum;
                //如果左右连接的都是同一个节点，置空右节点
                mapNodes[currentLevel][i].right = mapNodes[currentLevel][i].left == mapNodes[currentLevel][i].right ? null : mapNodes[currentLevel][i].right;
                firstNum = 0;
                secondNum = 0;
                canLeft = true;
                canRight = true;
            }
            // else
            // {
            //     mapNodes[currentLevel][i] = null;
            // }
        }
    }

    void ProcessLastLevel(int currentLevel)
    {
        // 設定最後一個 level 只有一個節點
        int lastNodeIndex = 2;
        mapNodes[currentLevel + 1][lastNodeIndex].isUsed = true;

        // 將前一個 level 的所有節點都連接到這個單一節點
        for (int i = 0; i < maxCount; i++)
        {
            if (mapNodes[currentLevel][i].isUsed)
            {
                mapNodes[currentLevel][i].left = mapNodes[currentLevel + 1][lastNodeIndex];
                mapNodes[currentLevel][i].right = null; // 確保只有一個連接
            }
        }
    }
    void CreateTheFirstLevel()
    {
        // int thisLevelCount = random.Next(maxCount);
        int thisLevelCount = 5;
        for (int i = 0; i < thisLevelCount; i++)
        {
            mapNodes[0][random.Next(maxCount)].isUsed = true;
        }
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
    // public void LogTrue()
    // {
    //     for (int i = 0; i < mapNodes.Length; i++)
    //     {
    //         for (int j = 0; j < mapNodes[0].Length; j++)
    //         {
    //             if (mapNodes[i][j] != null)
    //                 Debug.Log(i + "行" + j + "列" + mapNodes[i][j].isUsed);
    //         }
    //     }
    // }
    // public void SerTree()
    // {
    //     for (int i = 0; i < maxCount; i++)
    //     {
    //         if (mapNodes[0][i] != null)
    //         {
    //             Debug.Log(process(mapNodes[0][i]));
    //         }
    //     }
    // }
    // private String process(MapNode head)
    // {
    //     if (head == null)
    //         return "#!";
    //     String res = head.value + "!";
    //     res += process(head.left);
    //     res += process(head.right);
    //     return res;
    // }

    public void ShowRooms()
    {
        for (int i = 0; i < mapNodes.Length; i++)
        {
            for (int j = 0; j < mapNodes[0].Length; j++)
            {
                mapNodes[i][j].gameObject.SetActive(mapNodes[i][j].isUsed);
            }
        }
    }
    public void SetLevelsPosition()
    {
        for (int i = 0; i < levels.Length; i++)
        {
            int x = i * 150 + paddingX + leftPadding;
            int y = paddingY;
            levels[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
            levels[i].SetRoomsPosition(random);
        }
    }
    public void ShowLine()
    {
        for (int i = 0; i < mapNodes.Length; i++)
        {
            for (int j = 0; j < mapNodes[0].Length; j++)
            {
                if (mapNodes[i][j].isUsed)
                {
                    if (mapNodes[i][j].left != null)
                    {
                        GameObject line = new GameObject();
                        line.transform.SetParent(mapTrans);
                        LineRenderer ren = line.AddComponent<LineRenderer>();
                        ren.material = lineMaterial;
                        ren.useWorldSpace = false;
                        Vector3[] current = { mapNodes[i][j].transform.position, mapNodes[i][j].left.transform.position };
                        ren.SetPositions(current);
                    }
                    if (mapNodes[i][j].right != null)
                    {
                        GameObject line = new GameObject();
                        line.transform.SetParent(mapTrans);
                        LineRenderer ren = line.AddComponent<LineRenderer>();
                        ren.material = lineMaterial;
                        line.GetComponent<LineRenderer>().useWorldSpace = false;
                        Vector3[] current = { mapNodes[i][j].transform.position, mapNodes[i][j].right.transform.position };
                        ren.SetPositions(current);
                    }
                }
            }
        }
    }
}


