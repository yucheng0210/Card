using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Unity.VisualScripting;

public class BattleManager : Singleton<BattleManager>
{
    public enum BattleType
    {
        None,
        Explore,
        BattleInitial,
        DrawCard,
        Dialog,
        Player,
        Attack,
        UsingEffect,
        Enemy,
        Win,
        Victory,
        Loss
    }
    public enum ActionRangeType
    {
        None,
        Default,
        Linear,
        Surrounding,
        Cone,
        Jump,
        StraightCharge
    }
    public enum CheckEmptyType
    {
        PlayerAttack,
        EnemyAttack,
        Move,
        ALLCharacter
    }
    //玩家
    private int playerMoveCount;
    public int PlayerMoveCount
    {
        get { return playerMoveCount; }
        set
        {
            playerMoveCount = value;
            if (playerMoveCount > 2)
            {
                playerMoveCount = 2;
            }
        }
    }
    public int PlayerOnceMoveConsume { get; set; }
    public int CurrentDrawCardCount { get; set; }
    public BattleType MyBattleType { get; set; }
    public Transform CardMenuTrans { get; set; }
    public Transform CardBagTrans { get; set; }
    public Button CardBagApplyButton { get; set; }
    public CardItem CardPrefab { get; set; }
    //public List<CardItem> CardItemList { get; set; }
    //public List<Vector2> CardPositionList { get; set; }
    //public List<float> CardAngleList { get; set; }
    public Dictionary<string, int> CurrentNegativeState { get; set; }
    public Dictionary<string, int> CurrentAbilityList { get; set; }
    public Dictionary<string, string> CurrentTrapList { get; set; }
    public Dictionary<string, int> CurrentOnceBattlePositiveList { get; set; }
    public int ManaMultiplier { get; set; }
    public int CurrentConsumeMana { get; set; }
    public PlayerData CurrentPlayerData { get; set; }
    //敵人
    public Dictionary<string, EnemyData> CurrentEnemyList { get; set; }
    public Dictionary<string, EnemyData> CurrentMinionsList { get; set; }
    public Enemy EnemyPrefab { get; set; }
    public Transform EnemyTrans { get; set; }
    public bool IsDrag { get; set; }
    //棋盤
    public string CurrentLocationID { get; set; }
    public Dictionary<string, Terrain> CurrentTerrainList { get; set; }
    public Dictionary<string, string> CheckerboardList { get; set; }
    public RectTransform PlayerTrans { get; set; }
    public RectTransform CheckerboardTrans { get; set; }

    protected override void Awake()
    {
        base.Awake();
        IsDrag = false;
        //CardItemList = new List<CardItem>();
        /*CardPositionList = new List<Vector2>();
        CardAngleList = new List<float>();*/
        CurrentEnemyList = new Dictionary<string, EnemyData>();
        CurrentMinionsList = new Dictionary<string, EnemyData>();
        CurrentAbilityList = new Dictionary<string, int>();
        CheckerboardList = new Dictionary<string, string>();
        CurrentTerrainList = new Dictionary<string, Terrain>();
        CurrentNegativeState = new Dictionary<string, int>();
        CurrentTrapList = new Dictionary<string, string>();
        CurrentOnceBattlePositiveList = new Dictionary<string, int>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < CurrentEnemyList.Count; i++)
            {
                CharacterData value = CurrentEnemyList.ElementAt(i).Value;
                TakeDamage(CurrentPlayerData, value, 20, CurrentEnemyList.ElementAt(i).Key, 0);
            }
            /* for (int i = 0; i < CurrentEnemyList.Count; i++)
             {
                 Debug.Log(CurrentEnemyList.ElementAt(i).Key == CurrentEnemyList[CurrentEnemyList.ElementAt(i).Key].EnemyTrans.GetComponent<Enemy>().EnemyLocation);
             }*/
        }
    }
    public void TakeDamage(CharacterData attacker, CharacterData defender, int damage, string location, float delay)
    {
        StartCoroutine(TakeDamageCoroutine(attacker, defender, damage, location, delay));
    }
    private IEnumerator TakeDamageCoroutine(CharacterData attacker, CharacterData defender, int damage, string location, float delay)
    {
        // 等待指定的秒數
        yield return new WaitForSeconds(delay);

        // 執行原本的 TakeDamage 邏輯
        int currentDamage = damage * (100 - defender.DamageReduction) / 100 - defender.CurrentShield;
        if (currentDamage < 0)
        {
            currentDamage = 0;
        }
        defender.CurrentShield -= damage;
        defender.CurrentHealth -= currentDamage;

        int point = GetCheckerboardPoint(location);
        Vector2 pos = new(CheckerboardTrans.GetChild(point).localPosition.x, CheckerboardTrans.GetChild(point).localPosition.y);
        Color color = Color.red;
        EventManager.Instance.DispatchEvent(EventDefinition.eventTakeDamage, pos, damage, location, color, attacker, defender);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }

    public void Recover(CharacterData defender, int damage, string location)
    {
        defender.CurrentHealth += damage;
        int point = GetCheckerboardPoint(location);
        Vector2 pos = new(CheckerboardTrans.GetChild(point).localPosition.x, CheckerboardTrans.GetChild(point).localPosition.y);
        Color color = Color.green;
        EventManager.Instance.DispatchEvent(EventDefinition.eventTakeDamage, pos, damage, location, color);
    }
    public void Recover(CharacterData defender, int damage)
    {
        defender.CurrentHealth += damage;
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        Color color = Color.green;
        EventManager.Instance.DispatchEvent(EventDefinition.eventTakeDamage, screenCenter, damage, CurrentLocationID, color);
    }
    public void TriggerEnemyPassiveSkill(string locationID, bool isMinion)
    {
        EnemyData enemyData = isMinion ? CurrentMinionsList[locationID] : CurrentEnemyList[locationID];
        for (int i = 0; i < enemyData.PassiveSkills.Count; i++)
        {
            string key = enemyData.PassiveSkills.ElementAt(i).Key;
            EffectFactory.Instance.CreateEffect(key).ApplyEffect(enemyData.PassiveSkills[key], locationID, CurrentLocationID);
        }
    }
    public void SetEventTrigger(EventTrigger eventTrigger, UnityAction unityAction_1, UnityAction unityAction_2)
    {
        eventTrigger.triggers.Clear();
        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryExit.eventID = EventTriggerType.PointerExit;
        entryEnter.callback.AddListener((arg) => { unityAction_1(); });
        entryExit.callback.AddListener((arg) => { unityAction_2(); });
        eventTrigger.triggers.Add(entryEnter);
        eventTrigger.triggers.Add(entryExit);
    }
    public void ClearAllEventTriggers()
    {
        for (int i = 0; i < CheckerboardTrans.childCount; i++)
        {
            EventTrigger eventTrigger = CheckerboardTrans.GetChild(i).GetComponent<EventTrigger>();
            eventTrigger.triggers.Clear();
        }
    }
    public void GetShield(CharacterData defender, int point)
    {
        defender.CurrentShield += point;
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }

    public void ConsumeActionPoint(int point)
    {
        CurrentPlayerData.CurrentActionPoint -= point;
    }
    public int[] ConvertNormalPos(string location)
    {
        string[] myLocation = location.Split(' ');
        int[] normalPos = new int[2];
        normalPos[0] = int.Parse(myLocation[0]);
        normalPos[1] = int.Parse(myLocation[1]);
        return normalPos;
    }
    public string ConvertCheckerboardPos(int x, int y)
    {
        return x.ToString() + ' ' + y.ToString();
    }
    public int GetCheckerboardPoint(string point)
    {
        string[] points = point.Split(' ');
        return int.Parse(points[0]) + int.Parse(points[1]) * 8;
    }
    public string ConvertCheckerboardPos(int point)
    {
        int x = point % 8;
        int y = point / 8;
        return x.ToString() + ' ' + y.ToString();
    }
    private List<string> GetEmptyPlace(string location, int stepCount, CheckEmptyType checkEmptyType, bool isBFS)
    {
        List<string> emptyPlaceList = new();
        int[] pos = ConvertNormalPos(location);
        Vector2Int point = new Vector2Int(pos[0], pos[1]);
        int minX = point.x - stepCount;
        int maxX = point.x + stepCount;
        int minY = point.y - stepCount;
        int maxY = point.y + stepCount;
        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                string targetPos = ConvertCheckerboardPos(x, y);
                int testStepCount = GetRoute(location, targetPos, checkEmptyType).Count;
                // 跳過起始點
                if ((x == point.x && y == point.y) || testStepCount == 0)
                    continue;
                if ((testStepCount <= stepCount || !isBFS) && CheckPlaceEmpty(targetPos, checkEmptyType))
                    emptyPlaceList.Add(targetPos);
            }
        }
        return emptyPlaceList;
    }

    public bool CheckPlaceEmpty(string place, CheckEmptyType checkEmptyType)
    {
        if (!CheckerboardList.ContainsKey(place))
            return false;
        string placeStatus = CheckerboardList[place];
        bool playerAttackCondition = checkEmptyType == CheckEmptyType.PlayerAttack && placeStatus == "Enemy";
        bool enemyAttackCondition = checkEmptyType == CheckEmptyType.EnemyAttack && placeStatus == "Player";
        bool allCharacterCondition = checkEmptyType == CheckEmptyType.ALLCharacter && (placeStatus == "Player" || placeStatus == "Enemy");
        return playerAttackCondition || enemyAttackCondition || allCharacterCondition || placeStatus == "Empty";
    }
    public float CalculateDistance(string fromLocation, string toLocation)
    {
        int[] startPos = ConvertNormalPos(fromLocation);  // 從字符串位置轉換為座標
        int[] endPos = ConvertNormalPos(toLocation);

        int deltaX = endPos[0] - startPos[0];  // 計算 X 軸差異
        int deltaY = endPos[1] - startPos[1];  // 計算 Y 軸差異

        // 計算歐幾里得距離
        return Mathf.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }
    public List<string> GetAcitonRangeTypeList(string location, int stepCount, CheckEmptyType checkEmptyType, ActionRangeType actionRangeType)
    {
        List<string> emptyPlaceList = new List<string>();
        switch (actionRangeType)
        {
            case ActionRangeType.Linear:
                emptyPlaceList = GetLinearAttackList(location, CurrentLocationID); // 特定條件
                break;
            case ActionRangeType.Surrounding:
                emptyPlaceList = GetEmptyPlace(location, stepCount, checkEmptyType, false); // 特定條件
                break;
            case ActionRangeType.Cone:
                emptyPlaceList = GetConeAttackList(location, CurrentLocationID, stepCount); // 特定條件
                break;
            case ActionRangeType.Default:
                emptyPlaceList = GetEmptyPlace(location, stepCount, checkEmptyType, true);
                break;
            case ActionRangeType.Jump:
                emptyPlaceList = GetEmptyPlace(location, stepCount, checkEmptyType, true);
                break;
        }
        return emptyPlaceList;
    }
    public List<string> GetRoute(string fromLocation, string toLocation, CheckEmptyType checkEmptyType)
    {
        int[] startPos = ConvertNormalPos(fromLocation);
        int[] endPos = ConvertNormalPos(toLocation);

        // 存儲已經走過的格子
        HashSet<string> visited = new HashSet<string>();
        // 存儲路徑的佇列，每個佇列中的元素包括當前位置和該位置的路徑
        Queue<(int[] currentPos, List<string> path)> queue = new Queue<(int[], List<string>)>();

        // 初始化起始點
        queue.Enqueue((startPos, new List<string>()));
        visited.Add(fromLocation);

        // BFS遍歷格子
        while (queue.Count > 0)
        {
            var (currentPos, path) = queue.Dequeue();

            // 檢查是否已經到達終點
            if (currentPos[0] == endPos[0] && currentPos[1] == endPos[1])
            {
                return path; // 找到目標
            }

            // 定義可以移動的四個方向：上、下、左、右
            int[][] directions = new int[][]
            {
                new int[] { 0, 1 },  // 上
                new int[] { 0, -1 }, // 下
                new int[] { -1, 0 }, // 左
                new int[] { 1, 0 }   // 右
            };

            // 嘗試每個方向
            for (int i = 0; i < directions.Length; i++)
            {
                int[] nextPos = new int[] { currentPos[0] + directions[i][0], currentPos[1] + directions[i][1] };
                string nextLocation = ConvertCheckerboardPos(nextPos[0], nextPos[1]);

                // 檢查該位置是否已經訪問過，或者超出範圍
                if (!visited.Contains(nextLocation) && CheckerboardList.ContainsKey(nextLocation) && CheckPlaceEmpty(nextLocation, checkEmptyType))
                {
                    visited.Add(nextLocation); // 標記為已訪問
                    var newPath = new List<string>(path);
                    newPath.Add(nextLocation);
                    queue.Enqueue((nextPos, newPath));
                }
            }
        }

        // 若沒有找到路徑，則返回空列表
        return new List<string>();
    }
    public List<string> GetLinearAttackList(string fromLocation, string toLocation)
    {
        List<string> linearAttackList = new List<string>();

        // 轉換位置為座標
        int[] startPos = ConvertNormalPos(fromLocation);
        int[] endPos = ConvertNormalPos(toLocation);

        int startX = startPos[0];
        int startY = startPos[1];
        int endX = endPos[0];
        int endY = endPos[1];

        // 計算x和y軸的距離
        int distanceX = Mathf.Abs(endX - startX);
        int distanceY = Mathf.Abs(endY - startY);

        // 選擇距離較遠的軸，設置相應的方向與迴圈範圍
        bool isXPrimary = distanceX >= distanceY;
        int primaryStart = isXPrimary ? startX : startY;
        int primaryEnd = isXPrimary ? endX : endY;
        int secondaryPos = isXPrimary ? startY : startX;
        int direction = (primaryEnd > primaryStart) ? 1 : -1;

        // 擴展座標，檢查是否可以攻擊
        for (int pos = primaryStart + direction; ; pos += direction)
        {
            string newLocation = isXPrimary ? ConvertCheckerboardPos(pos, secondaryPos) : ConvertCheckerboardPos(secondaryPos, pos);

            if (CheckPlaceEmpty(newLocation, CheckEmptyType.EnemyAttack))
            {
                linearAttackList.Add(newLocation);
            }
            else
            {
                break;  // 如果有障礙物，停止擴展
            }
        }

        return linearAttackList;
    }

    public List<string> GetConeAttackList(string fromLocation, string toLocation, int attackDistance)
    {
        // 如果起點和終點相同，直接返回空列表
        if (fromLocation == toLocation)
            return null;

        List<string> coneAttackList = new List<string>();

        // 取得起點和目標點的座標
        int[] fromPos = ConvertNormalPos(fromLocation);
        int[] toPos = ConvertNormalPos(toLocation);

        // 計算x和y座標的差距來決定攻擊方向
        int dx = toPos[0] - fromPos[0];
        int dy = toPos[1] - fromPos[1];

        // 根據目標點相對起點的方位來確定攻擊方向
        int directionX = (dx != 0) ? (dx > 0 ? 1 : -1) : 0;  // x軸方向
        int directionY = (dy != 0) ? (dy > 0 ? 1 : -1) : 0;  // y軸方向

        bool prioritizeYDirection = directionY != 0;  // 是否以y方向為主
        int mainDirection = prioritizeYDirection ? directionY : directionX;

        // 生成等腰三角形範圍，根據攻擊距離
        for (int h = 0; h < attackDistance; h++)
        {
            // 計算對應高（h）位置的寬度範圍 (如 1, 3, 5...)
            int range = (2 * h) + 1;
            int offset = (h + 1) * mainDirection;  // 方向偏移量
            int halfRange = range / 2;  // 左右擴展範圍
            int breakCount = 0;
            for (int w = -halfRange; w <= halfRange; w++)
            {
                // 計算當前點的x和y座標
                int currentX = prioritizeYDirection ? fromPos[0] + w : fromPos[0] + offset;
                int currentY = prioritizeYDirection ? fromPos[1] + offset : fromPos[1] + w;

                // 將座標轉為棋盤位置
                string newLocation = ConvertCheckerboardPos(currentX, currentY);

                // 確認位置是否為空位且無障礙物
                if (CheckPlaceEmpty(newLocation, CheckEmptyType.EnemyAttack))
                {
                    coneAttackList.Add(newLocation);
                }
                else
                {
                    breakCount++;
                }
            }
            if (breakCount == range)
                break;
        }

        return coneAttackList;
    }

    public void RefreshCheckerboardList()
    {
        CheckerboardList.Clear();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                string location = ConvertCheckerboardPos(i, j);
                if (CurrentLocationID == location)
                {
                    CheckerboardList.Add(location, "Player");
                    //Debug.Log("玩家：" + location);
                }
                else if (CurrentEnemyList.ContainsKey(location))
                {
                    CheckerboardList.Add(location, "Enemy");
                    // Debug.Log("敵人：" + location);
                }
                else if (CurrentTerrainList.ContainsKey(location))
                {
                    CheckerboardList.Add(location, "Terrain");
                }
                else
                {
                    CheckerboardList.Add(location, "Empty");
                }
            }
        }
    }
    public void ConsumeMana(int consumeMana)
    {
        CurrentPlayerData.Mana -= consumeMana;
        CurrentConsumeMana += consumeMana;
    }
    public string AutomaticLineWrapping(string str, int wrappingCount)
    {
        string newStr = "";
        for (int i = 0; i < str.Length; i++)
        {
            newStr += str[i];
            if (i == wrappingCount - 1)
            {
                newStr += "\n\n";
            }
        }
        return newStr;
    }
    public void ChangeTurn(BattleType type)
    {
        MyBattleType = type;
        switch (MyBattleType)
        {
            case BattleType.None:
                //UIManager.Instance.HideUI("UIBattle");
                break;
            case BattleType.Explore:
                Explore();
                break;
            case BattleType.BattleInitial:
                BattleInitial();
                break;
            case BattleType.Attack:
                Attack();
                break;
            case BattleType.Dialog:
                Dialog();
                break;
            case BattleType.Player:
                PlayerTurn();
                break;
            case BattleType.Enemy:
                EnemyTurn();
                break;
            case BattleType.Win:
                Win();
                break;
            case BattleType.Loss:
                break;
        }
    }
    private void BattleInitial()
    {
        int levelID = MapManager.Instance.LevelID;
        int skillID = CurrentPlayerData.StartSkill;
        int levelCount = MapManager.Instance.LevelCount;
        PlayerOnceMoveConsume = 1;
        CurrentLocationID = MapManager.Instance.MapNodes[levelCount][levelID].l.PlayerStartPos;
        CurrentPlayerData.CurrentActionPoint = CurrentPlayerData.MaxActionPoint;
        CurrentPlayerData.Mana = 10;
        PlayerTrans.localPosition = CheckerboardTrans.GetChild(GetCheckerboardPoint(CurrentLocationID)).localPosition;
        CurrentDrawCardCount = CurrentPlayerData.DefaultDrawCardCount;
        for (int i = 0; i < DataManager.Instance.SkillList[skillID].SkillContent.Count; i++)
        {
            CurrentAbilityList.Add(DataManager.Instance.SkillList[skillID].SkillContent[i].Item1, DataManager.Instance.SkillList[skillID].SkillContent[i].Item2);
        }
        for (int i = 0; i < MapManager.Instance.MapNodes[levelCount][levelID].l.EnemyIDList.Count; i++)
        {
            int enemyID = MapManager.Instance.MapNodes[levelCount][levelID].l.EnemyIDList.ElementAt(i).Value;
            string loactionID = MapManager.Instance.MapNodes[levelCount][levelID].l.EnemyIDList.ElementAt(i).Key;
            CurrentEnemyList.Add(loactionID, DataManager.Instance.EnemyList[enemyID].DeepClone());
        }
        for (int i = 0; i < MapManager.Instance.MapNodes[levelCount][levelID].l.TerrainIDList.Count; i++)
        {
            int terrainID = MapManager.Instance.MapNodes[levelCount][levelID].l.TerrainIDList.ElementAt(i).Value;
            string locationID = MapManager.Instance.MapNodes[levelCount][levelID].l.TerrainIDList.ElementAt(i).Key;
            CurrentTerrainList.Add(locationID, DataManager.Instance.TerrainList[terrainID].Clone());
        }
        EventManager.Instance.DispatchEvent(EventDefinition.eventBattleInitial);
    }
    private void Attack()
    {
        //StartCoroutine(UIManager.Instance.RefreshEnemyAlert());
        RefreshCheckerboardList();
    }
    private void Explore()
    {
        string levelType = MapManager.Instance.MapNodes[MapManager.Instance.LevelCount][MapManager.Instance.LevelID].l.LevelType;
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
        EventManager.Instance.DispatchEvent(EventDefinition.eventExplore, levelType);
    }

    private void Dialog()
    {
        EventManager.Instance.DispatchEvent(EventDefinition.eventDialog);
    }

    private void PlayerTurn()
    {
        CurrentPlayerData.CurrentActionPoint = CurrentPlayerData.MaxActionPoint;
        CurrentPlayerData.CurrentShield = 0;
        for (int i = 0; i < CurrentAbilityList.Count; i++)
        {
            string effectID;
            int effectCount;
            effectID = CurrentAbilityList.ElementAt(i).Key;
            effectCount = CurrentAbilityList.ElementAt(i).Value;
            EffectFactory.Instance.CreateEffect(effectID).ApplyEffect(effectCount, CurrentLocationID, CurrentLocationID);
        }
        EventManager.Instance.DispatchEvent(EventDefinition.eventPlayerTurn);
    }

    private void EnemyTurn()
    {
        PlayerOnceMoveConsume = 1;
        for (int i = 0; i < CurrentNegativeState.Count; i++)
        {
            string negativeState = CurrentNegativeState.ElementAt(i).Key;
            CurrentNegativeState[negativeState]--;
            if (CurrentNegativeState[negativeState] <= 0)
                CurrentNegativeState.Remove(negativeState);
        }
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
        EventManager.Instance.DispatchEvent(EventDefinition.eventEnemyTurn);
    }

    private void Win()
    {
        EventManager.Instance.DispatchEvent(EventDefinition.eventBattleWin);
    }

    public void Shuffle()
    {
        List<CardData> cardBag = DataManager.Instance.CardBag;
        for (int i = 0; i < cardBag.Count; i++)
        {
            int randomIndex = Random.Range(0, cardBag.Count);
            CardData temp = cardBag[randomIndex];
            cardBag[randomIndex] = cardBag[i];
            cardBag[i] = temp;
        }
    }
    public void NextLevel(string hideMenu)
    {
        // BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Dialog);
        string showMenuStr = MapManager.Instance.LevelCount == 14 ? "UISkyIsland" : "UIMap";
        MapManager.Instance.LevelCount++;
        UIManager.Instance.ShowUI(showMenuStr);
        UIManager.Instance.HideUI(hideMenu);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }
    public CardItem AddCard(int id)
    {
        CardItem cardItem = Instantiate(CardPrefab, CardBagTrans);
        CardData cardData = DataManager.Instance.CardList[id].DeepClone();
        cardItem.transform.SetParent(CardBagTrans);
        RectTransform rectTransform = cardItem.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = CardBagTrans.position;
        cardItem.gameObject.SetActive(false);
        cardData.CardID = id;
        cardData.MyCardItem = cardItem;
        cardItem.MyCardData = cardData;
        DataManager.Instance.CardBag.Add(cardData);
        return cardItem;
    }
    public void AddMinions(int enemyID, int count, string location)
    {
        List<string> emptyPlaceList = GetEmptyPlace(location, 2, CheckEmptyType.Move, true);
        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, emptyPlaceList.Count);
            EnemyData enemyData = DataManager.Instance.EnemyList[enemyID].DeepClone();
            CurrentMinionsList.Add(emptyPlaceList[randomIndex], enemyData);
            emptyPlaceList.Remove(emptyPlaceList[randomIndex]);
        }
        for (int i = 0; i < CurrentMinionsList.Count; i++)
        {
            string key = CurrentMinionsList.ElementAt(i).Key;
            int checkerboardPoint = GetCheckerboardPoint(key);
            Enemy enemy = Instantiate(EnemyPrefab, EnemyTrans);
            enemy.GetComponent<RectTransform>().anchoredPosition = CheckerboardTrans.GetChild(checkerboardPoint).localPosition;
            enemy.EnemyID = CurrentMinionsList[key].CharacterID;
            enemy.EnemyImage.sprite = Resources.Load<Sprite>(CurrentMinionsList[key].EnemyImagePath);
            enemy.MyAnimator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(CurrentMinionsList[key].EnemyAniPath);
            CurrentMinionsList[key].EnemyTrans = enemy.GetComponent<RectTransform>();
            CurrentMinionsList[key].CurrentHealth = DataManager.Instance.EnemyList[enemy.EnemyID].MaxHealth;
            enemy.MyEnemyData = CurrentMinionsList[key];
            string minionsLocation = GetEnemyKey(CurrentMinionsList[key], CurrentMinionsList);
            TriggerEnemyPassiveSkill(minionsLocation, true);
        }
    }
    public int GetMinionsIDCount(int id)
    {
        int count = 0;
        for (int i = 0; i < CurrentEnemyList.Count; i++)
        {
            EnemyData enemyData = CurrentEnemyList.ElementAt(i).Value;
            if (enemyData.CharacterID == id)
                count++;
        }
        return count;
    }
    public int GetCardCount(int id)
    {
        int count = 0;
        List<CardData> cardBag = DataManager.Instance.CardBag;
        for (int i = 0; i < cardBag.Count; i++)
        {
            if (cardBag[i].CardID == id)
                count++;
        }
        List<CardData> usedCardBag = DataManager.Instance.UsedCardBag;
        for (int i = 0; i < usedCardBag.Count; i++)
        {
            if (usedCardBag[i].CardID == id)
                count++;
        }
        return count;
    }
    public CharacterData IdentifyCharacter(string location)
    {
        return !CheckerboardList.ContainsKey(location) ? null : CurrentEnemyList.ContainsKey(location) ? CurrentEnemyList[location] : CurrentPlayerData;
    }
    public string GetEnemyKey(EnemyData enemyData, Dictionary<string, EnemyData> enemyDataDict)
    {
        return enemyDataDict.FirstOrDefault(x => x.Value == enemyData).Key;
    }
}
