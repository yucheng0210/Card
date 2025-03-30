using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.Events;
//using Unity.VisualScripting;
using UnityEngine.TextCore.Text;
using System;
using UnityEngine.U2D;
using Cinemachine;
using Newtonsoft.Json.Linq;
using PilotoStudio;
using UnityEngine.Rendering;
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
        AfterEnemyAttack,
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
        SurroundingExplosion,
        Cone,
        Jump,
        StraightCharge,
        ThrowExplosion,
        ThrowScattering,
        AllZone
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
    public Transform UseCardBagTrans { get; set; }
    public Button CardBagApplyButton { get; set; }
    public CardItem CardPrefab { get; set; }
    public GameObject TrapPrefab { get; set; }
    public Transform TrapGroupTrans { get; set; }
    public CanvasGroup CharacterStatusClue { get; set; }
    public Dictionary<string, int> CurrentNegativeState { get; set; }
    public Dictionary<string, int> CurrentPositiveState { get; set; }
    public Dictionary<string, int> CurrentAbilityList { get; set; }
    public Dictionary<string, int> CurrentOnceBattlePositiveList { get; set; }
    public int ManaMultiplier { get; set; }
    public int CurrentConsumeMana { get; set; }
    public PlayerData CurrentPlayerData { get; set; }
    public Player CurrentPlayer { get; set; }
    public Animator PlayerAni { get; set; }
    public CardData InUseCardData { get; set; }
    //敵人
    public Dictionary<string, EnemyData> CurrentEnemyList { get; set; }
    public Dictionary<string, EnemyData> CurrentMinionsList { get; set; }
    public Enemy EnemyPrefab { get; set; }
    public Transform EnemyTrans { get; set; }
    public bool IsDrag { get; set; }
    public Material DissolveEdgeMaterial { get; set; }
    public Material DissolveMaterial { get; set; }
    public Material SpeedLineMaterial { get; set; }
    //棋盤
    public string CurrentPlayerLocation { get; set; }
    public Dictionary<string, Terrain> CurrentTerrainList { get; set; }
    public Dictionary<string, TrapData> CurrentTrapList { get; set; }
    public Dictionary<string, string> CheckerboardList { get; set; }
    public RectTransform PlayerTrans { get; set; }
    public RectTransform CheckerboardTrans { get; set; }
    public Texture2D DefaultCursor { get; set; }
    public Vector2 DefaultCursorHotSpot { get; set; }
    public int RoundCount { get; set; }
    public Volume GlobalVolume { get; set; }
    //bool i = false;
    protected override void Awake()
    {
        base.Awake();
        IsDrag = false;
        CurrentEnemyList = new Dictionary<string, EnemyData>();
        CurrentMinionsList = new Dictionary<string, EnemyData>();
        CurrentAbilityList = new Dictionary<string, int>();
        CheckerboardList = new Dictionary<string, string>();
        CurrentTerrainList = new Dictionary<string, Terrain>();
        CurrentNegativeState = new Dictionary<string, int>();
        CurrentPositiveState = new Dictionary<string, int>();
        CurrentTrapList = new();
        CurrentOnceBattlePositiveList = new Dictionary<string, int>();
        DefaultCursor = Resources.Load<Texture2D>("DefaultCursor_2");
        DefaultCursorHotSpot = new(20, 5);
        Cursor.SetCursor(DefaultCursor, DefaultCursorHotSpot, CursorMode.Auto);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            /*if (!i)
            {
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                i = !i;
            }
            else
            {
                Cursor.SetCursor(DefaultCursor, DefaultCursorHotSpot, CursorMode.Auto);
                i = !i;
            }*/
            /*CharacterData value = CurrentEnemyList.ElementAt(0).Value;
            TakeDamage(CurrentPlayerData, value, 51, CurrentEnemyList.ElementAt(0).Key, 0);*/
            //EventManager.Instance.DispatchEvent(EventDefinition.eventAfterMove);
            // PlayerMoveCount++;
            for (int i = 0; i < CurrentEnemyList.Count; i++)
            {
                CharacterData value = CurrentEnemyList.ElementAt(i).Value;
                TakeDamage(CurrentPlayerData, value, 50, CurrentEnemyList.ElementAt(i).Key, 0);
            }
            //TakeDamage(CurrentPlayerData, CurrentPlayerData, 50, CurrentPlayerLocation, 0);
            // playerMoveCount = 5;
            // StartCoroutine(SceneController.Instance.Transition("StartMenu"));
            /*  for (int i = 0; i < CurrentEnemyList.Count; i++)
              {
                  EnemyData enemyData = CurrentEnemyList.ElementAt(i).Value;
                  Enemy enemy = enemyData.EnemyTrans.GetComponent<Enemy>();
                  for (int j = 0; j < enemy.CurrentActionRangeTypeList.Count; j++)
                  {
                      Debug.Log(enemy.CurrentActionRangeTypeList[j]);
                  }
              }*/
        }
    }
    public void TakeDamage(CharacterData attacker, CharacterData defender, int damage, string location, float delay)
    {
        StartCoroutine(TakeDamageCoroutine(attacker, defender, damage, location, delay));
    }
    private IEnumerator TakeDamageCoroutine(CharacterData attacker, CharacterData defender, int damage, string location, float delay)
    {
        yield return new WaitForSeconds(delay);
        int randomDogeIndex = UnityEngine.Random.Range(0, 100);
        if (defender.DodgeChance > randomDogeIndex)
        {
            ShowCharacterStatusClue(((EnemyData)defender).EnemyTrans.GetComponent<Enemy>().StatusClueTrans, "閃避", 0);
            yield break;
        }
        int currentDamage = damage * (100 - defender.DamageReduction) / 100 - defender.CurrentShield;
        if (currentDamage < 0 || (CurrentOnceBattlePositiveList.ContainsKey("DamageImmunityEffect") && defender == CurrentPlayerData))
        {
            currentDamage = 0;
        }
        if (defender.DamageLimit >= 0 && currentDamage >= defender.DamageLimit)
        {
            currentDamage = defender.DamageLimit;
        }
        defender.CurrentShield -= damage;
        defender.CurrentHealth -= currentDamage;
        int point = GetCheckerboardPoint(location);
        Vector2 pos = new(CheckerboardTrans.GetChild(point).localPosition.x, CheckerboardTrans.GetChild(point).localPosition.y);
        Color color = Color.red;
        EventManager.Instance.DispatchEvent(EventDefinition.eventTakeDamage, pos, damage, location, color, attacker, defender, currentDamage);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }

    public void Recover(CharacterData defender, int damage, string location)
    {
        defender.CurrentHealth += damage;
        int point = GetCheckerboardPoint(location);
        Vector2 pos = new(CheckerboardTrans.GetChild(point).localPosition.x, CheckerboardTrans.GetChild(point).localPosition.y);
        Color color = Color.green;
        EventManager.Instance.DispatchEvent(EventDefinition.eventRecover, pos, damage, location, color);
    }
    public void Recover(CharacterData defender, int damage)
    {
        defender.CurrentHealth += damage;
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        Color color = Color.green;
        EventManager.Instance.DispatchEvent(EventDefinition.eventRecover, screenCenter, damage, CurrentPlayerLocation, color);
    }
    public void TriggerEnemyPassiveSkill(string location)
    {
        EnemyData enemyData = (EnemyData)IdentifyCharacter(location);
        Enemy enemy = enemyData.EnemyTrans.GetComponent<Enemy>();
        if (enemyData == null)
        {
            return;
        }
        for (int i = 0; i < enemyData.PassiveSkills.Count; i++)
        {
            string key = enemyData.PassiveSkills.ElementAt(i).Key;
            string clueStrs = EffectFactory.Instance.CreateEffect(key).SetTitleText();
            float waitTime = 0.5f * i;
            EffectFactory.Instance.CreateEffect(key).ApplyEffect(enemyData.PassiveSkills[key], location, CurrentPlayerLocation);
            ShowCharacterStatusClue(enemy.StatusClueTrans, clueStrs, waitTime);
        }
        enemyData.PassiveSkills.Clear();
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
    public string ConvertCheckerboardPos(int point)
    {
        int x = point % 8;
        int y = point / 8;
        return x.ToString() + ' ' + y.ToString();
    }
    public int GetCheckerboardPoint(string point)
    {
        string[] points = point.Split(' ');
        return int.Parse(points[0]) + int.Parse(points[1]) * 8;
    }
    public Transform GetCheckerboardTrans(string location)
    {
        return CheckerboardTrans.GetChild(GetCheckerboardPoint(location));
    }
    private List<string> GetEmptyPlace(string location, int stepCount, CheckEmptyType checkEmptyType, bool isBFS, bool isContainStartPos)
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
                string targetLocation = ConvertCheckerboardPos(x, y);
                int testStepCount = GetRoute(location, targetLocation, checkEmptyType).Count;
                // 跳過起始點
                if (targetLocation == location || testStepCount == 0)
                {
                    continue;
                }
                if ((testStepCount <= stepCount || !isBFS) && CheckPlaceEmpty(targetLocation, checkEmptyType))
                {
                    emptyPlaceList.Add(targetLocation);
                }
            }
        }
        if (isContainStartPos)
        {
            emptyPlaceList.Insert(0, location);
        }
        return emptyPlaceList;
    }
    public bool CheckPlaceEmpty(string place, CheckEmptyType checkEmptyType)
    {
        if (!CheckerboardList.ContainsKey(place))
        {
            return false;
        }
        string placeStatus = CheckerboardList[place];
        bool playerAttackCondition = checkEmptyType == CheckEmptyType.PlayerAttack && placeStatus == "Enemy";
        bool enemyAttackCondition = checkEmptyType == CheckEmptyType.EnemyAttack && placeStatus == "Player";
        bool allCharacterCondition = checkEmptyType == CheckEmptyType.ALLCharacter && (placeStatus == "Player" || placeStatus == "Enemy");
        bool moveCondition = checkEmptyType == CheckEmptyType.Move && placeStatus == "Trap";
        return playerAttackCondition || enemyAttackCondition || allCharacterCondition || moveCondition || placeStatus == "Empty";
    }
    public void CheckPlayerLocationInTrapRange()
    {
        if (CurrentTrapList.ContainsKey(CurrentPlayerLocation))
        {
            TrapData trapData = CurrentTrapList[CurrentPlayerLocation];
            for (int i = 0; i < trapData.TriggerSkillList.Count; i++)
            {
                string key = trapData.TriggerSkillList.ElementAt(i).Key;
                string clueStrs = EffectFactory.Instance.CreateEffect(key).SetTitleText();
                float waitTime = 0.5f * i;
                EffectFactory.Instance.CreateEffect(key).ApplyEffect(trapData.TriggerSkillList[key], CurrentPlayerLocation, CurrentPlayerLocation);
                ShowCharacterStatusClue(CurrentPlayer.StatusClueTrans, clueStrs, waitTime);
            }
            /*Animator ani = trapData.TrapTrans.GetComponent<Animator>();
            ani.SetTrigger("isAttacking");*/
            TakeDamage(CurrentPlayerData, CurrentPlayerData, trapData.CurrentAttack, CurrentPlayerLocation, 0.12f);
            CurrentTrapList.Remove(CurrentPlayerLocation);
            Destroy(trapData.TrapTrans.gameObject, 1);
        }
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
                    var newPath = new List<string>(path)
                    {
                        nextLocation
                    };
                    queue.Enqueue((nextPos, newPath));
                }
            }
        }

        // 若沒有找到路徑，則返回空列表
        return new List<string>();
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
    public List<string> GetActionRangeTypeList(string location, int stepCount, CheckEmptyType checkEmptyType, ActionRangeType actionRangeType)
    {
        List<string> emptyPlaceList = new();
        switch (actionRangeType)
        {
            case ActionRangeType.Linear:
                emptyPlaceList = GetLinearAttackList(location, CurrentPlayerLocation, stepCount); // 特定條件
                break;
            case ActionRangeType.Surrounding:
                emptyPlaceList = GetEmptyPlace(location, stepCount, checkEmptyType, false, false); // 特定條件
                break;
            case ActionRangeType.SurroundingExplosion:
                emptyPlaceList = GetEmptyPlace(location, stepCount, checkEmptyType, false, true); // 特定條件
                break;
            case ActionRangeType.Cone:
                emptyPlaceList = GetConeAttackList(location, CurrentPlayerLocation, stepCount); // 特定條件
                break;
            case ActionRangeType.Default:
                emptyPlaceList = GetEmptyPlace(location, stepCount, checkEmptyType, true, false);
                break;
            case ActionRangeType.StraightCharge:
                emptyPlaceList = GetStraightChargeList(location, CurrentPlayerLocation, stepCount);
                break;
            case ActionRangeType.Jump:
            case ActionRangeType.ThrowExplosion:
                emptyPlaceList = GetThrowExplosionList();
                break;
            case ActionRangeType.ThrowScattering:
                emptyPlaceList = GetThrowScatteringList(location);
                break;
            case ActionRangeType.AllZone:
                emptyPlaceList = GetAllZoneList(checkEmptyType);
                break;
        }
        return emptyPlaceList;
    }
    private List<string> GetAllZoneList(CheckEmptyType checkEmptyType)
    {
        List<string> emptyPlaceList = new List<string>();
        for (int i = 0; i < CheckerboardList.Count; i++)
        {
            string key = CheckerboardList.ElementAt(i).Key;
            if (!CheckPlaceEmpty(key, checkEmptyType))
            {
                continue;
            }
            emptyPlaceList.Add(key);
        }
        return emptyPlaceList;
    }
    public List<string> GetLinearAttackList(string fromLocation, string toLocation, int attackDistance)
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
        int count = 0;
        // 擴展座標，檢查是否可以攻擊
        for (int pos = primaryStart + direction; count < attackDistance; pos += direction)
        {
            string newLocation = isXPrimary ? ConvertCheckerboardPos(pos, secondaryPos) : ConvertCheckerboardPos(secondaryPos, pos);
            if (CheckPlaceEmpty(newLocation, CheckEmptyType.EnemyAttack))
            {
                linearAttackList.Add(newLocation);
                if (newLocation == toLocation)
                {
                    break;
                }
            }
            else
            {
                break;  // 如果有障礙物，停止擴展
            }
            count++;
        }

        return linearAttackList;
    }
    public List<string> GetConeAttackList(string fromLocation, string toLocation, int attackDistance)
    {
        List<string> coneAttackList = new List<string>();

        // 取得起點的座標
        int[] fromPos = ConvertNormalPos(fromLocation);

        // 四個方向 (上下左右) 的偏移向量
        int[,] directions = new int[,] { { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } };

        for (int d = 0; d < 4; d++) // 遍歷四個方向
        {
            int directionX = directions[d, 0];
            int directionY = directions[d, 1];

            for (int h = 0; h < attackDistance; h++) // 遍歷每一層距離
            {
                int range = (2 * h) + 1; // 寬度範圍 (如 1, 3, 5...)
                int offsetX = (h + 1) * directionX; // x方向偏移
                int offsetY = (h + 1) * directionY; // y方向偏移
                int halfRange = range / 2; // 左右擴展範圍
                int breakCount = 0;

                for (int w = -halfRange; w <= halfRange; w++) // 遍歷當層寬度
                {
                    // 計算當前點的x和y座標
                    int currentX = directionX != 0 ? fromPos[0] + offsetX : fromPos[0] + w;
                    int currentY = directionY != 0 ? fromPos[1] + offsetY : fromPos[1] + w;

                    // 將座標轉為棋盤位置
                    string newLocation = ConvertCheckerboardPos(currentX, currentY);

                    // 確認位置是否為空位且無障礙物
                    if (CheckPlaceEmpty(newLocation, CheckEmptyType.EnemyAttack))
                    {
                        if (!coneAttackList.Contains(newLocation)) // 避免重複
                        {
                            coneAttackList.Add(newLocation);
                        }
                    }
                    else
                    {
                        breakCount++;
                    }
                }

                if (breakCount == range)
                {
                    break; // 提前結束該層距離的計算
                }
            }
            if (coneAttackList.Contains(toLocation) || d == 3)
            {
                break;
            }
            else
            {
                coneAttackList.Clear();
            }
        }

        return coneAttackList;
    }
    private List<string> GetStraightChargeList(string fromLocation, string toLocation, int attackDistance)
    {
        List<string> emptyPlaceList = GetLinearAttackList(fromLocation, toLocation, attackDistance);
        List<string> straightChargeList = new List<string>();
        for (int i = 0; i < emptyPlaceList.Count; i++)
        {
            // 轉換座標字串為數值
            int[] currentPos = ConvertNormalPos(emptyPlaceList[i]);
            int x = currentPos[0];
            int y = currentPos[1];

            // 判定方向：根據起點與終點位置的相對座標來確定方向
            int[] fromPos = ConvertNormalPos(fromLocation);
            int[] toPos = ConvertNormalPos(toLocation);
            bool isHorizontal = Mathf.Abs(fromPos[0] - toPos[0]) >= Mathf.Abs(fromPos[1] - toPos[1]);

            // 根據方向選擇擴展座標
            string leftNeighbor, rightNeighbor;
            if (isHorizontal)
            {
                leftNeighbor = ConvertCheckerboardPos(x, y + 1);  // 左側座標
                rightNeighbor = ConvertCheckerboardPos(x, y - 1); // 右側座標
            }
            else
            {
                leftNeighbor = ConvertCheckerboardPos(x + 1, y);  // 上側座標
                rightNeighbor = ConvertCheckerboardPos(x - 1, y); // 下側座標
            }

            straightChargeList.Add(emptyPlaceList[i]);
            // 檢查座標是否符合條件
            if (CheckPlaceEmpty(leftNeighbor, CheckEmptyType.EnemyAttack))
            {
                straightChargeList.Add(leftNeighbor);
            }
            if (CheckPlaceEmpty(rightNeighbor, CheckEmptyType.EnemyAttack))
            {
                straightChargeList.Add(rightNeighbor);
            }
        }

        return straightChargeList;
    }
    private List<string> GetThrowExplosionList()
    {
        return GetActionRangeTypeList(CurrentPlayerLocation, 1, CheckEmptyType.EnemyAttack, ActionRangeType.SurroundingExplosion);
    }
    private List<string> GetThrowScatteringList(string fromLocation)
    {
        List<string> emptyPlaceList = GetActionRangeTypeList(CurrentPlayerLocation, 0, CheckEmptyType.EnemyAttack, ActionRangeType.AllZone);
        List<string> throwLocationList = new();
        Enemy enemy = CurrentEnemyList[fromLocation].EnemyTrans.GetComponent<Enemy>();
        int throwCount;
        if (enemy.MyActionType == Enemy.ActionType.Attack)
        {
            throwLocationList.Add(CurrentPlayerLocation);
            emptyPlaceList.Remove(CurrentPlayerLocation);
            throwCount = 20;
        }
        else
        {
            if (emptyPlaceList.Contains(CurrentPlayerLocation))
            {
                emptyPlaceList.Remove(CurrentPlayerLocation);
            }
            throwCount = 4;
        }
        //throwLocationList.Add(CurrentPlayerLocation);
        for (int i = 0; i < throwCount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, emptyPlaceList.Count);
            throwLocationList.Add(emptyPlaceList[randomIndex]);
            emptyPlaceList.RemoveAt(randomIndex);
        }
        return throwLocationList;
    }
    public string GetCloseLocation(string fromLocation, string toLocation, int attackDistance, CheckEmptyType checkEmptyType, ActionRangeType findType)
    {
        EnemyData enemyData = (EnemyData)IdentifyCharacter(fromLocation);
        Enemy enemy = enemyData.EnemyTrans.GetComponent<Enemy>();
        string bestInRangeLocation = null;
        int bestInRangeDistance = enemyData.MeleeAttackMode ? int.MaxValue : int.MinValue;
        List<string> emptyPlaceList = GetActionRangeTypeList(fromLocation, attackDistance, checkEmptyType, findType);
        string minLocation = emptyPlaceList[0];
        int minDistance = GetRoute(minLocation, toLocation, CheckEmptyType.ALLCharacter).Count;
        for (int j = 1; j < emptyPlaceList.Count; j++)
        {
            string targetLocation = emptyPlaceList[j];
            int targetDistance = GetRoute(targetLocation, toLocation, CheckEmptyType.ALLCharacter).Count;
            if (IsOtherLocationInRange(enemy, targetLocation))
            {
                bool bestInRangeCondition = enemyData.MeleeAttackMode == (targetDistance < bestInRangeDistance);
                if (bestInRangeCondition)
                {
                    bestInRangeLocation = targetLocation;
                    bestInRangeDistance = targetDistance;
                }
            }
            else if (bestInRangeLocation == null && targetDistance < minDistance)
            {
                minLocation = targetLocation;
                minDistance = targetDistance;
            }
        }
        return bestInRangeLocation ?? minLocation;
    }
    public void CheckPlayerLocationInRange(Enemy enemy)
    {
        string playerLocation = CurrentPlayerLocation;
        enemy.InRange = enemy.CurrentActionRangeTypeList.Contains(playerLocation) || enemy.MyNextAttackActionRangeType == ActionRangeType.None;
    }
    private bool IsOtherLocationInRange(Enemy enemy, string location)
    {
        string playerLocation = CurrentPlayerLocation;
        CheckEmptyType checkEmptyType = CheckEmptyType.EnemyAttack;
        int attackDistance = enemy.MyEnemyData.AttackRange;
        List<string> nextAttackRangeList = GetActionRangeTypeList(location, attackDistance, checkEmptyType, enemy.MyNextAttackActionRangeType);
        return nextAttackRangeList.Contains(playerLocation) || enemy.MyNextAttackActionRangeType == ActionRangeType.None;
    }
    public void RefreshCheckerboardList()
    {
        CheckerboardList.Clear();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                string location = ConvertCheckerboardPos(i, j);
                if (CurrentPlayerLocation == location)
                {
                    CheckerboardList.Add(location, "Player");
                    //Debug.Log("玩家：" + location);
                }
                else if (CurrentEnemyList.ContainsKey(location) || CurrentMinionsList.ContainsKey(location))
                {
                    CheckerboardList.Add(location, "Enemy");
                    // Debug.Log("敵人：" + location);
                }
                else if (CurrentTerrainList.ContainsKey(location))
                {
                    CheckerboardList.Add(location, "Terrain");
                }
                else if (CurrentTrapList.ContainsKey(location))
                {
                    CheckerboardList.Add(location, "Trap");
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
            case BattleType.UsingEffect:
                UsingEffect();
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
            case BattleType.AfterEnemyAttack:
                AfterEnemyAttack();
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
        List<int> skillList = CurrentPlayerData.StartSkillList;
        int levelCount = MapManager.Instance.LevelCount;
        RoundCount = 0;
        PlayerOnceMoveConsume = 1;
        CurrentPlayerLocation = MapManager.Instance.MapNodes[levelCount][levelID].l.PlayerStartPos;
        CurrentPlayerData.CurrentActionPoint = CurrentPlayerData.MaxActionPoint;
        CurrentPlayerData.Mana = 10;
        CurrentPlayerData.CurrentActionPoint = CurrentPlayerData.MaxActionPoint;
        PlayerMoveCount = 2;
        PlayerTrans.localPosition = CheckerboardTrans.GetChild(GetCheckerboardPoint(CurrentPlayerLocation)).localPosition;
        CurrentDrawCardCount = CurrentPlayerData.DefaultDrawCardCount;
        for (int j = 0; j < skillList.Count; j++)
        {
            Skill skill = DataManager.Instance.SkillList[skillList[j]];
            if (skill.SkillType == "戰鬥")
            {
                for (int i = 0; i < skill.SkillContent.Count; i++)
                {
                    string effectID;
                    int effectCount;
                    effectID = skill.SkillContent.ElementAt(i).Key;
                    effectCount = skill.SkillContent[effectID];
                    EffectFactory.Instance.CreateEffect(effectID).ApplyEffect(effectCount, CurrentPlayerLocation, CurrentPlayerLocation);
                }
                continue;
            }
            for (int i = 0; i < skill.SkillContent.Count; i++)
            {
                CurrentAbilityList.Add(skill.SkillContent.ElementAt(i).Key, skill.SkillContent[skill.SkillContent.ElementAt(i).Key]);
            }
        }
        for (int i = 0; i < MapManager.Instance.MapNodes[levelCount][levelID].l.EnemyIDList.Count; i++)
        {
            int enemyID = MapManager.Instance.MapNodes[levelCount][levelID].l.EnemyIDList.ElementAt(i).Value;
            string location = MapManager.Instance.MapNodes[levelCount][levelID].l.EnemyIDList.ElementAt(i).Key;
            if (DataManager.Instance.EnemyList[enemyID].IsMinion)
            {
                CurrentMinionsList.Add(location, DataManager.Instance.EnemyList[enemyID].DeepClone());
            }
            else
            {
                CurrentEnemyList.Add(location, DataManager.Instance.EnemyList[enemyID].DeepClone());
            }
        }
        for (int i = 0; i < MapManager.Instance.MapNodes[levelCount][levelID].l.TerrainIDList.Count; i++)
        {
            int terrainID = MapManager.Instance.MapNodes[levelCount][levelID].l.TerrainIDList.ElementAt(i).Value;
            string locationID = MapManager.Instance.MapNodes[levelCount][levelID].l.TerrainIDList.ElementAt(i).Key;
            CurrentTerrainList.Add(locationID, DataManager.Instance.TerrainList[terrainID].Clone());
        }
        RefreshCheckerboardList();
        EventManager.Instance.DispatchEvent(EventDefinition.eventBattleInitial);
    }
    private void Attack()
    {
        RefreshCheckerboardList();
        EventManager.Instance.DispatchEvent(EventDefinition.eventAttack);
    }
    private void UsingEffect()
    {
        SwitchHandCardRaycast(false);
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
        ManaMultiplier = 1;
        CurrentConsumeMana = 0;
        PlayerMoveCount++;
        RoundCount++;
        CurrentPlayerData.CurrentActionPoint = CurrentPlayerData.MaxActionPoint;
        CurrentPlayerData.CurrentShield = 0;
        CurrentPlayerData.DamageReduction = 0;
        for (int i = 0; i < CurrentAbilityList.Count; i++)
        {
            string effectID;
            int effectCount;
            effectID = CurrentAbilityList.ElementAt(i).Key;
            effectCount = CurrentAbilityList.ElementAt(i).Value;
            EffectFactory.Instance.CreateEffect(effectID).ApplyEffect(effectCount, CurrentPlayerLocation, CurrentPlayerLocation);
        }
        for (int i = 0; i < CurrentPositiveState.Count; i++)
        {
            string positiveState = CurrentPositiveState.ElementAt(i).Key;
            ReducePositiveState(positiveState);
        }
        EventManager.Instance.DispatchEvent(EventDefinition.eventPlayerTurn);
    }

    private void EnemyTurn()
    {
        PlayerOnceMoveConsume = 1;
        for (int i = 0; i < CurrentNegativeState.Count; i++)
        {
            string negativeState = CurrentNegativeState.ElementAt(i).Key;
            ReduceNegativeState(negativeState);
        }
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
        EventManager.Instance.DispatchEvent(EventDefinition.eventEnemyTurn);
    }
    private void AfterEnemyAttack()
    {
        EventManager.Instance.DispatchEvent(EventDefinition.eventAfterEnemyAttack);
    }
    public void ReduceNegativeState(string negativeState)
    {
        CurrentNegativeState[negativeState]--;
        if (CurrentNegativeState[negativeState] <= 0)
        {
            CurrentNegativeState.Remove(negativeState);
        }
    }
    public void ReducePositiveState(string positiveState)
    {
        CurrentPositiveState[positiveState]--;
        if (CurrentPositiveState[positiveState] <= 0)
        {
            CurrentPositiveState.Remove(positiveState);
        }
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
            int randomIndex = UnityEngine.Random.Range(0, cardBag.Count);
            CardData temp = cardBag[randomIndex];
            cardBag[randomIndex] = cardBag[i];
            cardBag[i] = temp;
        }
    }
    public void NextLevel(string hideMenu)
    {
        // BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Dialog);
        string showMenuStr = MapManager.Instance.LevelCount == 14 ? "UISkyIsland" : "UIMap";
        if (MapManager.Instance.ChapterCount == 3)
        {
            showMenuStr = "UIGameOver";
            EventManager.Instance.DispatchEvent(EventDefinition.eventGameOver, true);
            EventManager.Instance.DispatchEvent(EventDefinition.eventReloadGame);
        }
        MapManager.Instance.LevelCount++;
        UIManager.Instance.ShowUI(showMenuStr);
        UIManager.Instance.HideUI(hideMenu);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }
    public CardItem AddCard(int id)
    {
        CardItem cardItem = Instantiate(CardPrefab, CardBagTrans);
        CardData cardData = DataManager.Instance.CardList[id].DeepClone();
        RectTransform rectTransform = cardItem.GetComponent<RectTransform>();
        cardItem.transform.SetParent(CardBagTrans);
        rectTransform.anchoredPosition = CardBagTrans.position;
        cardItem.gameObject.SetActive(false);
        cardData.CardID = id;
        cardData.MyCardItem = cardItem;
        cardItem.MyCardData = cardData;
        int randomIndex = UnityEngine.Random.Range(0, DataManager.Instance.CardBag.Count);
        DataManager.Instance.CardBag.Insert(randomIndex, cardData);
        return cardItem;
    }
    public void AddState(Dictionary<string, int> stateList, string stateName, int stateValue)
    {
        if (!stateList.ContainsKey(stateName))
        {
            stateList.Add(stateName, stateValue);
        }
        else
        {
            stateList[stateName] += stateValue;
        }
        if (stateList[stateName] <= 0)
        {
            stateList.Remove(stateName);
        }
    }
    public void AddMinions(int enemyID, int count, string location)
    {
        List<string> emptyPlaceList = GetEmptyPlace(location, count, CheckEmptyType.Move, true, false);
        for (int i = 0; i < count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, emptyPlaceList.Count);
            EnemyData enemyData = DataManager.Instance.EnemyList[enemyID].DeepClone();
            enemyData.CurrentHealth = enemyData.MaxHealth;
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
            enemy.MasterEnemyData = CurrentEnemyList[location];
            CurrentMinionsList[key].EnemyTrans = enemy.GetComponent<RectTransform>();
            enemy.MyEnemyData = CurrentMinionsList[key];
            enemy.MyNextAttackActionRangeType = ActionRangeType.None;
            SetEnemyAttackPower(enemy, enemy.MyEnemyData);
        }
        EventManager.Instance.DispatchEvent(EventDefinition.eventMove);
    }
    public void AddTrap(List<string> trapList, int id, string fromLocation)
    {
        for (int i = 0; i < trapList.Count;)
        {
            int randomIndex = UnityEngine.Random.Range(0, trapList.Count);
            RectTransform trapRect = Instantiate(TrapPrefab, TrapGroupTrans).GetComponent<RectTransform>();
            TrapData trapData = DataManager.Instance.TrapList[id].DeepClone();
            float distance = CalculateDistance(fromLocation, trapList[randomIndex]);
            int curveHeight = 250;
            trapRect.localPosition = GetCheckerboardTrans(fromLocation).localPosition;
            Vector2 startPoint = trapRect.localPosition;
            Vector2 endPoint = GetCheckerboardTrans(trapList[randomIndex]).localPosition;
            Vector2 midPoint = new(startPoint.x + distance / 2, startPoint.y + curveHeight);
            trapRect.GetComponent<Image>().sprite = Resources.Load<Sprite>(trapData.TrapImagePath);
            Tween moveTween = DOTween.To((t) =>
            {
                Vector2 position = UIManager.Instance.GetBezierCurve(startPoint, midPoint, endPoint, t);
                trapRect.anchoredPosition = position;
            }, 0, 1, 1.25f).SetEase(Ease.InQuad);
            trapData.CurrentHealth = trapData.MaxHealth;
            trapData.CurrentAttack = trapData.BaseAttack;
            trapData.TrapTrans = trapRect;
            CurrentTrapList.Add(trapList[randomIndex], trapData);
            trapList.RemoveAt(randomIndex);
        }
    }
    public int GetMinionsIDCount(int id)
    {
        int count = 0;
        for (int i = 0; i < CurrentMinionsList.Count; i++)
        {
            EnemyData enemyData = CurrentMinionsList.ElementAt(i).Value;
            if (enemyData.CharacterID == id)
            {
                count++;
            }
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
            {
                count++;
            }
        }
        List<CardData> usedCardBag = DataManager.Instance.UsedCardBag;
        for (int i = 0; i < usedCardBag.Count; i++)
        {
            if (usedCardBag[i].CardID == id)
            {
                count++;
            }
        }
        return count;
    }
    public CharacterData IdentifyCharacter(string location)
    {
        if (!CheckerboardList.ContainsKey(location))
        {
            return null;
        }

        if (CurrentEnemyList.TryGetValue(location, out EnemyData enemy))
        {
            return enemy;
        }

        if (CurrentMinionsList.TryGetValue(location, out EnemyData minion))
        {
            return minion;
        }
        return location == CurrentPlayerLocation ? CurrentPlayerData : null;
    }
    public string GetEnemyKey(EnemyData enemyData)
    {
        string key = CurrentEnemyList.FirstOrDefault(x => x.Value == enemyData).Key;
        key ??= CurrentMinionsList.FirstOrDefault(x => x.Value == enemyData).Key;
        return key;
    }
    public void Replace<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TKey oldKey, TKey newKey)
    {
        TValue value = dictionary[oldKey];
        dictionary.Remove(oldKey);
        dictionary.Add(newKey, value);
        RefreshCheckerboardList();
    }
    public void ShowCharacterStatusClue(Transform trans, string des, float waitTime)
    {
        StartCoroutine(ShowCharacterStatusClueCoroutine(trans, des, waitTime));
    }
    private IEnumerator ShowCharacterStatusClueCoroutine(Transform trans, string des, float waitTime)
    {
        for (int i = 0; i < trans.childCount; i++)
        {
            if (trans.GetChild(i).GetComponent<Text>().text == des)
            {
                yield break;
            }
        }
        if (trans.childCount > 6)
        {
            yield break;
        }
        if (trans.childCount > 0)
        {
            yield return new WaitForSeconds(waitTime);
        }
        // 創建新提示
        CanvasGroup clue = Instantiate(CharacterStatusClue, trans);
        RectTransform clueRect = clue.GetComponent<RectTransform>();
        Text clueText = clue.GetComponent<Text>();
        // 獲取上一個子物件的終點位置
        if (trans.childCount > 1)
        {
            RectTransform lastChildRect = trans.GetChild(trans.childCount - 2).GetComponent<RectTransform>();
            clueRect.anchoredPosition = lastChildRect.anchoredPosition + new Vector2(0, 25); // 終點向上偏移
        }
        else
        {
            clueRect.anchoredPosition = Vector2.zero; // 預設位置
        }

        // 動畫和文字設定
        DG.Tweening.Sequence sequence = DOTween.Sequence();
        clueText.text = des;

        // 動畫序列
        sequence.Append(clueRect.DOLocalMoveY(clueRect.anchoredPosition.y + 75, 0.5f)).AppendCallback(() => StartCoroutine(UIManager.Instance.FadeIn(clue, 1f, true)));
    }
    public void ThrowAwayHandCard(List<CardItem> throwAwayList, float moveTime)
    {
        for (int i = 0; i < throwAwayList.Count; i++)
        {
            CardItem cardItem = throwAwayList[i];
            CardData cardData = cardItem.MyCardData;
            RectTransform cardItemRect = cardItem.GetComponent<RectTransform>();
            RectTransform useCardBagRect = UseCardBagTrans.GetComponent<RectTransform>();
            DataManager.Instance.UsedCardBag.Add(cardData);
            cardItem.CantMove = true;
            cardItemRect.DOAnchorPos(useCardBagRect.anchoredPosition, moveTime);
            EventManager.Instance.DispatchEvent(EventDefinition.eventUseCard, cardItem);
        }
        for (int i = 0; i < throwAwayList.Count; i++)
        {
            CardItem cardItem = throwAwayList[i];
            CardData cardData = cardItem.MyCardData;
            DataManager.Instance.HandCard.Remove(cardData);
        }
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }
    public void ClearAllMinions()
    {
        for (int i = 0; i < CurrentMinionsList.Count; i++)
        {
            EnemyData value = CurrentMinionsList.ElementAt(i).Value;
            Enemy enemy = value.EnemyTrans.GetComponent<Enemy>();
            enemy.MyAnimator.SetTrigger("isDeath");
            EventManager.Instance.ClearEvents(value);
            Destroy(enemy.gameObject, 1);
            EventManager.Instance.DispatchEvent(EventDefinition.eventMove);
        }
        CurrentMinionsList.Clear();
    }
    public void RemoveMinion(EnemyData enemyData)
    {
        string location = GetEnemyKey(enemyData);
        Enemy enemy = enemyData.EnemyTrans.GetComponent<Enemy>();
        enemy.MyAnimator.SetTrigger("isDeath");
        CurrentMinionsList.Remove(location);
        EventManager.Instance.ClearEvents(enemyData);
        Destroy(enemy.gameObject, 1);
        EventManager.Instance.DispatchEvent(EventDefinition.eventMove);

    }
    public void SwitchHandCardRaycast(bool switchBool)
    {
        List<CardData> handCard = DataManager.Instance.HandCard;
        for (int i = 0; i < handCard.Count; i++)
        {
            handCard[i].MyCardItem.CardImage.raycastTarget = switchBool;
        }
    }
    public void ExchangePos(Transform transA, Dictionary<string, EnemyData> enemyListA, string locationA, Transform transB, string locationB, Dictionary<string, EnemyData> enemyListB)
    {
        Vector3 tempPosition = transA.localPosition;
        transA.localPosition = transB.localPosition;
        transB.localPosition = tempPosition;
        EnemyData dataA = enemyListA[locationA];
        EnemyData dataB = enemyListB[locationB];
        Enemy dataAEnemy = dataA.EnemyTrans.GetComponent<Enemy>();
        Enemy dataBEnemy = dataB.EnemyTrans.GetComponent<Enemy>();
        enemyListA.Remove(locationA);
        enemyListB.Remove(locationB);
        enemyListA[locationB] = dataA;
        enemyListB[locationA] = dataB;
        ShowCharacterStatusClue(dataAEnemy.StatusClueTrans, "轉移", 0);
        ShowCharacterStatusClue(dataBEnemy.StatusClueTrans, "轉移", 0);
        EventManager.Instance.DispatchEvent(EventDefinition.eventMove);
    }
    public void TemporaryChangeEffect(Enemy enemy, string effectName, string targetLocation)
    {
        string[] effectNames = effectName.Split('=');
        enemy.InfoTitle.text = "效果";
        enemy.InfoDescription.text = "施展未知效果。";
        enemy.EnemyAttackIntentText.text = "";
        enemy.ResetUIElements();
        enemy.EnemyEffectImage.SetActive(true);
        Sprite effectSprite = EffectFactory.Instance.CreateEffect(effectNames[0]).SetIcon();
        if (effectSprite == null)
        {
            effectSprite = EffectFactory.Instance.CreateEffect("KnockBackEffect").SetIcon();
        }
        enemy.EnemyEffectImage.GetComponent<Image>().sprite = effectSprite;
        enemy.MyActionType = Enemy.ActionType.Effect;
        enemy.TemporaryEffect = effectName;
        enemy.NoNeedCheckInRange = true;
        enemy.TargetLocation = targetLocation;
        int effectRange = EffectFactory.Instance.CreateEffect(effectNames[0]).SetEffectRange();
        ActionRangeType actionRangeType = EffectFactory.Instance.CreateEffect(effectNames[0]).SetEffectAttackType();
        GetActionRangeTypeList(GetEnemyKey(enemy.MyEnemyData), effectRange, CheckEmptyType.EnemyAttack, actionRangeType);
        enemy.CurrentActionRangeTypeList = GetActionRangeTypeList(GetEnemyKey(enemy.MyEnemyData), effectRange, CheckEmptyType.EnemyAttack, actionRangeType);
        enemy.MyEnemyData.CurrentAttackOrderIndex--;
        CheckPlayerLocationInRange(enemy);
    }
    public void TemporaryChangeAttack(Enemy enemy, string targetLocation, List<string> actionRangeTypeList, int attackCount)
    {
        enemy.InfoTitle.text = "攻擊";
        enemy.InfoDescription.text = "發動攻擊。";
        enemy.EnemyAttackIntentText.text = "";
        enemy.ResetUIElements();
        enemy.EnemyAttackImage.SetActive(true);
        enemy.TargetLocation = targetLocation;
        SetEnemyAttackIntentText(enemy);
        enemy.MyActionType = Enemy.ActionType.Attack;
        enemy.MyNextAttackActionRangeType = ActionRangeType.AllZone;
        enemy.CurrentAttackCount = attackCount;
        enemy.CurrentActionRangeTypeList = actionRangeTypeList;
        enemy.MyEnemyData.CurrentAttackOrderIndex--;
        CheckPlayerLocationInRange(enemy);
    }
    public void TemporaryChangeShield(Enemy enemy, int shieldCount)
    {
        enemy.InfoTitle.text = "護盾";
        enemy.InfoDescription.text = "產生護盾。";
        enemy.EnemyAttackIntentText.text = "";
        enemy.ResetUIElements();
        enemy.EnemyShieldImage.SetActive(true);
        enemy.EnemyAttackIntentText.text = shieldCount.ToString();
        enemy.MyActionType = Enemy.ActionType.Shield;
        enemy.MyNextAttackActionRangeType = ActionRangeType.None;
        enemy.CurrentActionRangeTypeList.Clear();
        enemy.EnemyAttackIntentText.enabled = false;
        enemy.CurrentShieldCount = shieldCount;
        enemy.MyEnemyData.CurrentAttackOrderIndex--;
        CheckPlayerLocationInRange(enemy);
    }
    public void SetEnemyAttackPower(Enemy enemy, EnemyData enemyData)
    {
        if (enemy.CurrentAttackCount == 1)
        {
            enemy.CurrentAttackPower = enemyData.MaxAttack * (1 + enemy.AdditionAttackMultiplier) + enemy.AdditionPower;
            return;
        }
        int attackPower = Mathf.RoundToInt(enemyData.MaxAttack / (enemy.CurrentAttackCount * 0.75f));
        enemy.CurrentAttackPower = Mathf.Clamp(attackPower, enemyData.MinAttack, enemyData.MaxAttack) * (1 + enemy.AdditionAttackMultiplier) + enemy.AdditionPower;
    }
    public void SetEnemyAttackIntentText(Enemy enemy)
    {
        SetEnemyAttackPower(enemy, enemy.MyEnemyData);
        int totalAttackCount = enemy.CurrentAttackCount + enemy.AdditionAttackCount;
        string attackCountStrs = totalAttackCount == 1 ? "" : "X" + totalAttackCount;
        enemy.EnemyAttackIntentText.text = enemy.CurrentAttackPower.ToString() + attackCountStrs;
    }
    public int GetPercentage(int maxCount, int percentage)
    {
        int count = percentage >= 0 ? Mathf.RoundToInt(maxCount * (percentage / 100f)) : -1;
        return count;
    }
    public void SetParticleEffectCoroutine(Vector3 startPos, Vector3 endPos, string particlePath)
    {
        StartCoroutine(SetParticleEffect(startPos, endPos, particlePath));
    }
    public IEnumerator SetParticleEffect(Animator ani, Vector3 startPos, Vector3 endPos, string particlePath, bool isWaitAnimation)
    {
        if (isWaitAnimation)
        {
            ani.SetTrigger("isAttacking");
        }
        while (isWaitAnimation)
        {
            AnimatorStateInfo stateInfo = ani.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsTag("Attack") && stateInfo.normalizedTime >= 0.5f)
            {
                break;
            }
            yield return null;
        }
        ParticleEffectIDCard particleEffect = Resources.Load<ParticleEffectIDCard>(particlePath);
        switch (particleEffect.MyAttackType)
        {
            case ParticleEffectIDCard.AttackType.Melee:
                Instantiate(particleEffect, endPos, Quaternion.identity);
                break;
            case ParticleEffectIDCard.AttackType.LongDistance:
                ProjectileController projectileController = Instantiate(particleEffect, startPos, Quaternion.identity).GetComponent<ProjectileController>();
                projectileController.Destination = endPos;
                Sequence sequence = projectileController.AttackSequence();
                yield return sequence.WaitForCompletion();
                break;
        }
        GameObject blood = Resources.Load<GameObject>("ParticleEffect/HitBlood");
        Instantiate(blood, endPos, Quaternion.identity);
        AudioManager.Instance.SEAudio(particleEffect.SoundIndex);
    }
    public IEnumerator SetParticleEffect(Vector3 startPos, Vector3 endPos, string particlePath)
    {
        ParticleEffectIDCard particleEffect = Resources.Load<ParticleEffectIDCard>(particlePath);
        switch (particleEffect.MyAttackType)
        {
            case ParticleEffectIDCard.AttackType.Melee:
                Instantiate(particleEffect, endPos, Quaternion.identity);
                break;
            case ParticleEffectIDCard.AttackType.LongDistance:
                ProjectileController projectileController = Instantiate(particleEffect, startPos, Quaternion.identity).GetComponent<ProjectileController>();
                projectileController.Destination = endPos;
                Sequence sequence = projectileController.AttackSequence();
                yield return sequence.WaitForCompletion();
                break;
        }
        GameObject blood = Resources.Load<GameObject>("ParticleEffect/HitBlood");
        Instantiate(blood, endPos, Quaternion.identity);
        AudioManager.Instance.SEAudio(particleEffect.SoundIndex);
    }
    public void SetDissolveMaterial(Material dissolveMaterial, float startValue, float endValue, TweenCallback callBackTween)
    {
        Sequence sequence = DOTween.Sequence();
        float progress = startValue;
        Tween tween = DOTween.To(() => progress, x =>
        {
            progress = x;
            dissolveMaterial.SetFloat("_Progress", progress);
        }, endValue, 1.0f).SetEase(Ease.OutQuad);
        sequence.Append(tween).Pause();
        sequence.AppendCallback(callBackTween);
        sequence.Play();
    }
    public bool CheckEnemyInAttackRange(string enemyLocation, int attackDistance)
    {
        string id = CurrentPlayerLocation;
        CheckEmptyType checkEmptyType = CheckEmptyType.PlayerAttack;
        ActionRangeType actionRangeType = ActionRangeType.Default;
        List<string> emptyPlaceList = GetActionRangeTypeList(id, attackDistance, checkEmptyType, actionRangeType);
        return emptyPlaceList.Contains(enemyLocation);
    }
}