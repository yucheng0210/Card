using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using UnityEngine.EventSystems;

public class UIBattle : UIBase
{
    [SerializeField]
    private List<EventTrigger> battleInfoList = new List<EventTrigger>();
    [Header("玩家")]
    [SerializeField]
    private Text actionPointText;

    [SerializeField]
    private Text manaPointText;

    [SerializeField]
    private Text shieldText;

    [SerializeField]
    private Image health;

    [SerializeField]
    private Text healthText;

    [SerializeField]
    private Text battleCardBagCountText;

    [SerializeField]
    private Text usedCardBagCountText;
    [SerializeField]
    private Text removeCardBagCountText;

    [SerializeField]
    private Button changeTurnButton;
    [SerializeField]
    private Button playerMoveButton;
    [SerializeField]
    private Transform playerMoveGridTrans;
    [SerializeField]
    private Transform negativeGroupTrans;
    [SerializeField]
    private Transform positiveGroupTrans;
    [SerializeField]
    private BattleState negativePrefab;

    [Header("敵人")]
    [SerializeField]
    private Enemy enemyPrefab;

    [SerializeField]
    private Transform enemyTrans;
    [SerializeField]
    private GameObject enemyInfo;
    [SerializeField]
    private Text enemyName;
    [SerializeField]
    private Image enemyImage;
    [SerializeField]
    private Text enemyShield;
    [SerializeField]
    private Text enemyHealth;
    [SerializeField]
    private Transform enemyPassiveGroupTrans;

    [Header("傷害特效")]
    [SerializeField]
    private GameObject damageNumPrefab;

    [SerializeField]
    private float xOffset;

    [SerializeField]
    private float curveHeight;
    [Header("棋盤")]
    [SerializeField]
    private RectTransform checkerboardTrans;

    [SerializeField]
    private GameObject terrainPrefab;

    [SerializeField]
    private Transform terrainTrans;
    [Header("藥水")]
    [SerializeField]
    private Transform potionGroupTrans;
    [SerializeField]
    private Button potionPrefab;
    [SerializeField]
    private Transform potionClueMenu;

    protected override void Start()
    {
        base.Start();
        changeTurnButton.onClick.AddListener(ChangeTurn);
        playerMoveButton.onClick.AddListener(PlayerMove);
        BattleManager.Instance.CheckerboardTrans = checkerboardTrans;
        EventManager.Instance.AddEventRegister(EventDefinition.eventRefreshUI, EventRefreshUI);
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
        EventManager.Instance.AddEventRegister(EventDefinition.eventBattleInitial, EventBattleInitial);
        EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
        EventManager.Instance.AddEventRegister(EventDefinition.eventEnemyTurn, EventEnemyTurn);
        EventManager.Instance.AddEventRegister(EventDefinition.eventMove, EventMove);
        //Hide();
       StartGame();
    }

    private void StartGame()
    {
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
        BattleManager.Instance.EnemyPrefab = enemyPrefab;
        BattleManager.Instance.EnemyTrans = enemyTrans;
        CheckBattleInfo();
        Hide();
    }
    private void CheckBattleInfo()
    {
        for (int i = 0; i < battleInfoList.Count; i++)
        {
            int id = i;
            battleInfoList[id].triggers.Clear();
            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;
            entryExit.eventID = EventTriggerType.PointerExit;
            entryEnter.callback.AddListener((arg) => { battleInfoList[id].transform.GetChild(0).gameObject.SetActive(true); });
            entryExit.callback.AddListener((arg) => { battleInfoList[id].transform.GetChild(0).gameObject.SetActive(false); });
            battleInfoList[id].triggers.Add(entryEnter);
            battleInfoList[id].triggers.Add(entryExit);
        }
    }
    private void CheckEnemyInfo()
    {
        UIManager.Instance.ClearMoveClue(false);
        for (int i = 0; i < checkerboardTrans.childCount; i++)
        {
            string location = BattleManager.Instance.ConvertCheckerboardPos(i);
            if (!BattleManager.Instance.CurrentEnemyList.ContainsKey(location))
                continue;
            EventTrigger eventTrigger = checkerboardTrans.GetChild(i).GetComponent<EventTrigger>();
            EnemyData enemyData = BattleManager.Instance.CurrentEnemyList[location];
            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;
            entryExit.eventID = EventTriggerType.PointerExit;
            entryEnter.callback.AddListener((arg) => { RefreshEnemyInfo(location); });
            entryExit.callback.AddListener((arg) =>
            {
                if (BattleManager.Instance.MyBattleType != BattleManager.BattleType.Attack)
                    return;
                UIManager.Instance.ClearCheckerboardColor(location, enemyData.StepCount, BattleManager.CheckEmptyType.EnemyAttack);
            });
            eventTrigger.triggers.Add(entryEnter);
            eventTrigger.triggers.Add(entryExit);
            //checkerboardTrans.GetChild(i).GetComponent<Button>().onClick.AddListener(() => RefreshEnemyInfo(location));
        }
    }
    private void ClearAllEventTriggers()
    {
        for (int i = 0; i < checkerboardTrans.childCount; i++)
        {
            EventTrigger eventTrigger = checkerboardTrans.GetChild(i).GetComponent<EventTrigger>();
            eventTrigger.triggers.Clear();
        }
    }
    private void RefreshEnemyInfo(string location)
    {
        if (BattleManager.Instance.MyBattleType != BattleManager.BattleType.Attack)
            return;
        float distance = BattleManager.Instance.GetDistance(location);
        EnemyData enemyData = BattleManager.Instance.CurrentEnemyList[location];
        if (distance <= enemyData.AttackDistance)
            UIManager.Instance.ChangeCheckerboardColor(false, location, enemyData.AttackDistance, BattleManager.CheckEmptyType.EnemyAttack);
        else
            UIManager.Instance.ChangeCheckerboardColor(true, location, enemyData.StepCount, BattleManager.CheckEmptyType.EnemyAttack);
        enemyInfo.SetActive(true);
        enemyName.text = enemyData.CharacterName;
        enemyImage.sprite = Resources.Load<Sprite>(enemyData.EnemyImagePath);
        enemyHealth.text = enemyData.CurrentHealth.ToString() + "/" + enemyData.MaxHealth.ToString();
        enemyShield.text = enemyData.CurrentShield.ToString();
        for (int i = 0; i < enemyPassiveGroupTrans.childCount; i++)
        {
            Destroy(enemyPassiveGroupTrans.GetChild(i).gameObject);
        }
        for (int i = 0; i < enemyData.MaxPassiveSkillsList.Count; i++)
        {
            Image passive = Instantiate(negativePrefab, enemyPassiveGroupTrans).BattleStateImage;
            Sprite image = EffectFactory.Instance.CreateEffect(enemyData.MaxPassiveSkillsList[i]).SetIcon();
            passive.sprite = image;
        }
    }

    private void ChangeTurn()
    {
        if (BattleManager.Instance.MyBattleType != BattleManager.BattleType.Attack)
            return;
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Enemy);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }
    private void PlayerMove()
    {
        bool playerCantMove = BattleManager.Instance.PlayerMoveCount <= 0;
        bool notInAttack = BattleManager.Instance.MyBattleType != BattleManager.BattleType.Attack;
        bool containsCantMoveEffect = BattleManager.Instance.CurrentNegativeState.ContainsKey(nameof(CantMoveEffect));
        if (playerCantMove || notInAttack || containsCantMoveEffect)
            return;
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.UsingEffect);
        EffectFactory.Instance.CreateEffect("MoveEffect").ApplyEffect(BattleManager.Instance.PlayerMoveCount, BattleManager.Instance.CurrentLocationID);
    }
    private void RemoveEnemy(string key)
    {
        EnemyData enemyData = BattleManager.Instance.CurrentEnemyList[key];
        Enemy enemy = enemyData.EnemyTrans.GetComponent<Enemy>();
        if (enemyData.CurrentHealth <= 0 && !enemy.IsDeath)
        {
            enemy.MyAnimator.SetTrigger("isDeath");
            if (!enemyData.PassiveSkills.ContainsKey("ResurrectionEffect"))
            {
                BattleManager.Instance.CurrentEnemyList.Remove(key);
                Destroy(enemyData.EnemyTrans.gameObject, 1);
                ClearAllEventTriggers();
                // CheckBattleInfo();
                CheckEnemyInfo();
            }
            enemy.IsDeath = true;
        }
        else
            enemy.MyAnimator.SetTrigger("isHited");
        BattleManager.Instance.RefreshCheckerboardList();
        if (BattleManager.Instance.CurrentEnemyList.Count == 0)
        {
            BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Win);
            BattleManager.Instance.CurrentNegativeState.Clear();
            BattleManager.Instance.CurrentEnemyList.Clear();
            BattleManager.Instance.CurrentAbilityList.Clear();
            BattleManager.Instance.CurrentTerrainList.Clear();
            BattleManager.Instance.CurrentOnceBattlePositiveList.Clear();
            BattleManager.Instance.CurrentMinionsList.Clear();
            RemoveAllTerrian();
        }
    }
    private void RemoveAllTerrian()
    {
        for (int i = 0; i < terrainTrans.childCount; i++)
        {
            Destroy(terrainTrans.GetChild(i).gameObject);
        }
    }
    private void EventBattleInitial(params object[] args)
    {
        BattleInitial();
    }

    private void BattleInitial()
    {
        RefreshPotionBag();

        // 获取当前的敌人列表和地形列表
        var enemyList = BattleManager.Instance.CurrentEnemyList;
        var terrainList = BattleManager.Instance.CurrentTerrainList;
        var checkerboardTrans = BattleManager.Instance.CheckerboardTrans;
        // 处理敌人列表
        for (int i = 0; i < enemyList.Count; i++)
        {
            string key = enemyList.ElementAt(i).Key;
            EnemyData enemyData = enemyList.ElementAt(i).Value;

            int checkerboardPoint = BattleManager.Instance.GetCheckerboardPoint(key);
            Enemy enemy = Instantiate(enemyPrefab, enemyTrans);
            RectTransform enemyRect = enemy.GetComponent<RectTransform>();

            enemy.EnemyLocation = key;
            enemyRect.anchoredPosition = checkerboardTrans.GetChild(checkerboardPoint).localPosition;
            enemy.EnemyID = enemyData.CharacterID;
            enemy.EnemyImage.sprite = Resources.Load<Sprite>(enemyData.EnemyImagePath);
            enemy.MyAnimator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(enemyData.EnemyAniPath);

            enemyData.CurrentAttack = enemyData.MinAttack;
            enemyData.EnemyTrans = enemyRect;
            enemyData.CurrentHealth = DataManager.Instance.EnemyList[enemy.EnemyID].MaxHealth;

            BattleManager.Instance.TriggerEnemyPassiveSkill(enemy.EnemyLocation, false);
        }

        // 处理地形列表
        for (int i = 0; i < terrainList.Count; i++)
        {
            string key = terrainList.ElementAt(i).Key;
            Terrain terrainData = terrainList.ElementAt(i).Value;

            GameObject terrain = Instantiate(terrainPrefab, terrainTrans);
            RectTransform terrainRect = terrain.GetComponent<RectTransform>();
            Image terrainImage = terrain.GetComponent<Image>();

            terrainImage.sprite = Resources.Load<Sprite>(terrainData.ImagePath);
            terrainRect.anchoredPosition = checkerboardTrans.GetChild(BattleManager.Instance.GetCheckerboardPoint(key)).localPosition;
        }

        // 初始化玩家回合
        BattleManager.Instance.PlayerMoveCount = 2;
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Player);
    }

    private void RefreshPotionBag()
    {
        for (int i = 0; i < potionGroupTrans.childCount; i++)
        {
            Destroy(potionGroupTrans.GetChild(i).gameObject);
        }
        for (int i = 0; i < DataManager.Instance.PotionBag.Count; i++)
        {
            int avoidClosure = i;
            Button potion = Instantiate(potionPrefab, potionGroupTrans);
            potion.onClick.AddListener(() => UsePotion(DataManager.Instance.PotionBag[avoidClosure].ItemID, avoidClosure));
        }
    }
    private void UsePotion(int itemID, int bagID)
    {
        if (BattleManager.Instance.MyBattleType == BattleManager.BattleType.Enemy)
            return;
        potionClueMenu.gameObject.SetActive(true);
        string[] effect = DataManager.Instance.ItemList[itemID].ItemEffectName.Split('=');
        string effectName = effect[0];
        int value = int.Parse(effect[1]);
        Button yesButton = potionClueMenu.GetChild(0).GetComponent<Button>();
        Button noButton = potionClueMenu.GetChild(1).GetComponent<Button>();
        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(() => UsePotionEffect(effectName, value, bagID));
        noButton.onClick.AddListener(() => potionClueMenu.gameObject.SetActive(false));
    }
    private void UsePotionEffect(string effectName, int value, int bagID)
    {
        EffectFactory.Instance.CreateEffect(effectName).ApplyEffect(value, BattleManager.Instance.CurrentLocationID);
        DataManager.Instance.PotionBag.RemoveAt(bagID);
        potionClueMenu.gameObject.SetActive(false);
        RefreshPotionBag();
        EventRefreshUI();
    }
    private void EventPlayerTurn(params object[] args)
    {
        BattleManager.Instance.ManaMultiplier = 1;
        BattleManager.Instance.CurrentConsumeMana = 0;
        BattleManager.Instance.PlayerMoveCount++;
        ClearAllEventTriggers();
        CheckEnemyInfo();
        //  CheckBattleInfo();
    }
    private void EventEnemyTurn(params object[] args)
    {
        UIManager.Instance.ClearMoveClue(true);
        ClearAllEventTriggers();
    }
    private void EventMove(params object[] args)
    {
        BattleManager.Instance.RefreshCheckerboardList();
        UIManager.Instance.ClearMoveClue(true);
        ClearAllEventTriggers();
        CheckEnemyInfo();
        // CheckBattleInfo();
    }
    private void EventTakeDamage(params object[] args)
    {
        string locationID = (string)args[2];
        if (BattleManager.Instance.CurrentLocationID != locationID)
        {
            RemoveEnemy(locationID);
            if (BattleManager.Instance.CurrentEnemyList.ContainsKey(locationID))
                BattleManager.Instance.TriggerEnemyPassiveSkill(locationID, false);
        }

        GameObject damageNum = Instantiate(damageNumPrefab, UI.transform);
        RectTransform damageRect = damageNum.GetComponent<RectTransform>();
        Text damageText = damageNum.GetComponentInChildren<Text>();
        damageText.text = args[1].ToString();
        Color color = (Color)args[3];
        damageText.DOColor(color, 0.75f);
        xOffset *= -1;
        Vector2 startPoint = (Vector2)args[0];
        Vector2 endPoint = new(startPoint.x + xOffset, -540);
        Vector2 midPoint = new(startPoint.x + xOffset / 2, startPoint.y + curveHeight);
        Vector2 endScale = new(0.5f, 0.5f);
        DOTween.To((t) =>
        {
            Vector2 position = UIManager.Instance.GetBezierCurve(startPoint, midPoint, endPoint, t);
            damageRect.anchoredPosition = position;
        }, 0, 1, 1).SetEase(Ease.OutQuad);
        damageRect.DOScale(endScale, 1);
        StartCoroutine(UIManager.Instance.FadeOutIn(damageNum.GetComponent<CanvasGroup>(), 1f, 0, true));
    }

    private void EventRefreshUI(params object[] args)
    {
        // 提取局部变量
        var playerData = BattleManager.Instance.CurrentPlayerData;
        var cardBag = DataManager.Instance.CardBag;
        var usedCardBag = DataManager.Instance.UsedCardBag;
        var removeCardBag = DataManager.Instance.RemoveCardBag;
        var negativeState = BattleManager.Instance.CurrentNegativeState;
        var positiveList = BattleManager.Instance.CurrentOnceBattlePositiveList;

        // 更新UI文本
        actionPointText.text = $"{playerData.CurrentActionPoint}/{playerData.MaxActionPoint}";
        manaPointText.text = playerData.Mana.ToString();
        health.DOFillAmount((float)playerData.CurrentHealth / playerData.MaxHealth, 0.5f);
        healthText.text = $"{playerData.CurrentHealth}/{playerData.MaxHealth}";
        shieldText.text = playerData.CurrentShield.ToString();
        battleCardBagCountText.text = cardBag.Count.ToString();
        usedCardBagCountText.text = usedCardBag.Count.ToString();
        removeCardBagCountText.text = removeCardBag.Count.ToString();

        // 更新玩家移动网格
        int playerMoveCount = BattleManager.Instance.PlayerMoveCount;
        for (int i = 0; i < playerMoveGridTrans.childCount; i++)
        {
            playerMoveGridTrans.GetChild(i).gameObject.SetActive(i < playerMoveCount);
        }

        // 更新负面状态
        UpdateStateUI(negativeGroupTrans, negativeState, negativePrefab);

        // 更新正面状态
        UpdateStateUI(positiveGroupTrans, positiveList, negativePrefab);
    }

    private void UpdateStateUI(Transform groupTrans, Dictionary<string, int> stateList, BattleState prefab)
    {
        // 清空当前状态
        for (int i = groupTrans.childCount - 1; i >= 0; i--)
        {
            Destroy(groupTrans.GetChild(i).gameObject);
        }

        // 重新生成状态图标
        for (int i = 0; i < stateList.Count; i++)
        {
            string key = stateList.ElementAt(i).Key;
            int value = stateList[key];
            BattleState stateObject = Instantiate(prefab, groupTrans);
            Image stateImage = stateObject.BattleStateImage;
            Text stateText = stateObject.BattleStateAmount;
            Text infoTitle = stateObject.InfoTitle;
            Text infoDescription = stateObject.InfoDescription;
            Transform infoGroupTrans = stateObject.InfoGroupTrans;
            stateImage.sprite = EffectFactory.Instance.CreateEffect(key).SetIcon();
            stateText.text = value.ToString();
            EventTrigger eventTrigger = infoGroupTrans.GetComponent<EventTrigger>();
            eventTrigger.triggers.Clear();
            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;
            entryExit.eventID = EventTriggerType.PointerExit;
            infoTitle.text = EffectFactory.Instance.CreateEffect(key).SetTitleText();
            infoDescription.text = EffectFactory.Instance.CreateEffect(key).SetDescriptionText();
            entryEnter.callback.AddListener((arg) => { infoGroupTrans.GetChild(0).gameObject.SetActive(true); });
            entryExit.callback.AddListener((arg) => { infoGroupTrans.GetChild(0).gameObject.SetActive(false); });
            eventTrigger.triggers.Add(entryEnter);
            eventTrigger.triggers.Add(entryExit);
        }
    }

}