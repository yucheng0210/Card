using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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
    private Transform skillStateGroupTrans;
    [SerializeField]
    private Transform negativeGroupTrans;
    [SerializeField]
    private Transform positiveGroupTrans;
    [SerializeField]
    private BattleState negativeLPrefab;
    [SerializeField]
    private BattleState negativeRPrefab;

    [Header("敵人")]
    [SerializeField]
    private Material dissolveEdgeMaterial;
    [SerializeField]
    private Material dissolveMaterial;
    [SerializeField]
    private Material speedLineMaterial;
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
    [SerializeField]
    private Transform enemyStateGroupTrans;

    [Header("傷害特效")]
    [SerializeField]
    private GameObject damageNumPrefab;

    [SerializeField]
    private float xOffset;

    [SerializeField]
    private float curveHeight;
    [SerializeField]
    private CanvasGroup bloodScreenCanvasGroup;
    [SerializeField]
    private Volume volume;
    [SerializeField]
    private float maxVignetteIntensity;
    [SerializeField]
    private float firstBloodScreenDuration;
    [SerializeField]
    private float secondBloodScreenDuration;
    [Header("棋盤")]
    [SerializeField]
    private RectTransform checkerboardTrans;

    [SerializeField]
    private GameObject terrainPrefab;

    [SerializeField]
    private Transform terrainTrans;
    [SerializeField]
    private CanvasGroup roundTip;
    [SerializeField]
    private Sprite playerRound;
    [SerializeField]
    private Sprite enemyRound;
    [SerializeField]
    private Button speedupButton;
    [SerializeField]
    private CanvasGroup characterStatusClue;
    [SerializeField]
    private Image battleBackground;
    [SerializeField]
    private List<Sprite> battleBackgroundSpriteList;
    [Header("陷阱")]
    [SerializeField]
    private GameObject trapPrefab;
    [SerializeField]
    private Transform trapGroupTrans;
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
        EventManager.Instance.AddEventRegister(EventDefinition.eventRefreshUI, EventRefreshUI);
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
        EventManager.Instance.AddEventRegister(EventDefinition.eventRecover, EventRecover);
        EventManager.Instance.AddEventRegister(EventDefinition.eventBattleInitial, EventBattleInitial);
        EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
        EventManager.Instance.AddEventRegister(EventDefinition.eventEnemyTurn, EventEnemyTurn);
        EventManager.Instance.AddEventRegister(EventDefinition.eventMove, EventMove);
        EventManager.Instance.AddEventRegister(EventDefinition.eventAfterMove, EventAfterMove);
        EventManager.Instance.AddEventRegister(EventDefinition.eventDrawCard, EventDrawCard);
        EventManager.Instance.AddEventRegister(EventDefinition.eventNextChapter, EventNextChapter);
        //Hide();
        StartGame();
    }

    private void StartGame()
    {
        battleBackground.sprite = battleBackgroundSpriteList[MapManager.Instance.ChapterCount - 1];
        for (int i = 0; i < BattleManager.Instance.CurrentPlayerData.StartSkillList.Count; i++)
        {
            int skillID = BattleManager.Instance.CurrentPlayerData.StartSkillList[i];
            if (DataManager.Instance.SkillList[skillID].IsTalentSkill)
            {
                BattleManager.Instance.PlayerAni.runtimeAnimatorController = DataManager.Instance.SkillList[skillID].TalentAnimatorController;
            }
        }
        speedLineMaterial.color = new Color(speedLineMaterial.color.r, speedLineMaterial.color.g, speedLineMaterial.color.b, 0);
        BattleManager.Instance.GlobalVolume = volume;
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
        BattleManager.Instance.EnemyPrefab = enemyPrefab;
        BattleManager.Instance.EnemyTrans = enemyTrans;
        BattleManager.Instance.TrapPrefab = trapPrefab;
        BattleManager.Instance.TrapGroupTrans = trapGroupTrans;
        BattleManager.Instance.CheckerboardTrans = checkerboardTrans;
        BattleManager.Instance.CharacterStatusClue = characterStatusClue;
        BattleManager.Instance.DissolveEdgeMaterial = dissolveEdgeMaterial;
        BattleManager.Instance.DissolveMaterial = dissolveMaterial;
        BattleManager.Instance.SpeedLineMaterial = speedLineMaterial;
        Hide();
    }
    private void CheckBattleInfo()
    {
        for (int i = 0; i < battleInfoList.Count; i++)
        {
            int id = i;
            UnityAction unityAction_1 = () => { battleInfoList[id].transform.GetChild(0).gameObject.SetActive(true); };
            UnityAction unityAction_2 = () => { battleInfoList[id].transform.GetChild(0).gameObject.SetActive(false); };
            BattleManager.Instance.SetEventTrigger(battleInfoList[id], unityAction_1, unityAction_2);
        }
    }
    private void CheckEnemyInfo()
    {
        BattleManager.Instance.ClearAllEventTriggers();
        UIManager.Instance.ClearMoveClue(false);
        for (int i = 0; i < checkerboardTrans.childCount; i++)
        {
            string location = BattleManager.Instance.ConvertCheckerboardPos(i);
            if (!BattleManager.Instance.CurrentEnemyList.ContainsKey(location) && !BattleManager.Instance.CurrentMinionsList.ContainsKey(location))
            {
                continue;
            }
            EventTrigger eventTrigger = checkerboardTrans.GetChild(i).GetComponent<EventTrigger>();
            UnityAction unityAction_1 = () =>
            {
                RefreshEnemyInfo(location);
            };
            UnityAction unityAction_2 = () =>
            {
                if (BattleManager.Instance.MyBattleType != BattleManager.BattleType.Attack)
                {
                    return;
                }
                EnemyData enemyData = (EnemyData)BattleManager.Instance.IdentifyCharacter(location);
                Enemy enemy = enemyData.EnemyTrans.GetComponent<Enemy>();
                enemy.AttackAimingClue.SetActive(false);
                UIManager.Instance.ClearMoveClue(false);
            };
            BattleManager.Instance.SetEventTrigger(eventTrigger, unityAction_1, unityAction_2);
            BattleManager.Instance.TriggerEnemyPassiveSkill(location);
        }
    }

    private void RefreshEnemyInfo(string location)
    {
        if (BattleManager.Instance.MyBattleType != BattleManager.BattleType.Attack)
        {
            return;
        }
        EnemyData enemyData = (EnemyData)BattleManager.Instance.IdentifyCharacter(location);
        if (enemyData == null)
        {
            return;
        }
        Enemy enemy = enemyData.EnemyTrans.GetComponent<Enemy>();
        CardData cardData = BattleManager.Instance.InUseCardData;
        if (cardData != null && BattleManager.Instance.CheckEnemyInAttackRange(location, cardData.CardAttackDistance))
        {
            enemy.AttackAimingClue.SetActive(true);
        }
        bool isMove = enemy.MyActionType == Enemy.ActionType.Move;
        UIManager.Instance.ChangeCheckerboardColor(enemy.CurrentActionRangeTypeList, isMove);
        enemyInfo.SetActive(true);
        enemyName.text = enemyData.CharacterName;
        enemyImage.sprite = Resources.Load<Sprite>(enemyData.EnemyImagePath);
        enemyHealth.text = enemyData.CurrentHealth.ToString() + "/" + enemyData.MaxHealth.ToString();
        enemyShield.text = enemyData.CurrentShield.ToString();
        UpdateStateUI(enemyPassiveGroupTrans, enemyData.MaxPassiveSkillsList, negativeLPrefab, true, true);
        UpdateStateUI(enemyStateGroupTrans, enemy.EnemyOnceBattlePositiveList, negativeLPrefab, false, true);
    }
    private IEnumerator EnemyAttack()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        Dictionary<string, EnemyData> currentEnemyList = BattleManager.Instance.CurrentEnemyList;
        Dictionary<string, EnemyData> currentMinionsList = BattleManager.Instance.CurrentMinionsList;
        List<EnemyData> moveHistoryList = new();

        // 將敵人的攻擊邏輯提取到單獨的方法中
        yield return StartCoroutine(ExecuteEnemyActions(currentMinionsList, moveHistoryList));
        yield return new WaitForSecondsRealtime(2);
        yield return StartCoroutine(ExecuteEnemyActions(currentEnemyList, moveHistoryList));
        yield return new WaitForSecondsRealtime(2);
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.AfterEnemyAttack);
        yield return null;
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Player);
    }

    private IEnumerator ExecuteEnemyActions(Dictionary<string, EnemyData> enemyList, List<EnemyData> moveHistoryList)
    {
        for (int i = 0; i < enemyList.Count; i++)
        {
            var kvp = enemyList.ElementAt(i);
            string location = kvp.Key;
            EnemyData enemyData = kvp.Value;
            if (moveHistoryList.Contains(enemyData))  // 跳过已经移动过的敌人
            {
                continue;
            }
            RectTransform enemyTrans = enemyData.EnemyTrans;
            Enemy enemy = enemyTrans.GetComponent<Enemy>();
            Image enemyImage = enemy.EnemyImage;
            PlayerData playerData = BattleManager.Instance.CurrentPlayerData;
            SetEnemyAttackRotation(enemyData, enemyImage, location);  // 设置敌人朝向
            switch (enemy.MyActionType)
            {
                case Enemy.ActionType.Move:
                    yield return HandleEnemyMove(location, enemyData, enemy, enemyImage, enemyList);  // 单独处理移动逻辑
                    moveHistoryList.Add(enemyData);
                    break;
                case Enemy.ActionType.Attack:
                    int attackCount = enemy.CurrentAttackCount + enemy.AdditionAttackCount;
                    yield return HandleEnemyAttack(enemyData, enemy, playerData, attackCount); // 单独处理攻击逻辑
                    break;
                case Enemy.ActionType.Shield:
                    BattleManager.Instance.GetShield(enemyData, enemy.CurrentShieldCount);  // 处理护盾
                    break;
                case Enemy.ActionType.Effect:
                    yield return ApplyEffect(enemy, enemyData, location);  // 处理效果
                    break;
            }
            UpdateAttackOrder(enemyData, enemy);  // 更新攻击顺序
            EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }
    // 设置敌人朝向
    private void SetEnemyAttackRotation(EnemyData enemyData, Image enemyImage, string location)
    {
        int locationX = BattleManager.Instance.ConvertNormalPos(location)[0];
        int playerLocationX = BattleManager.Instance.ConvertNormalPos(BattleManager.Instance.CurrentPlayerLocation)[0];
        bool shouldFlip = (playerLocationX > locationX) ? enemyData.ImageFlip : !enemyData.ImageFlip;
        enemyImage.transform.localRotation = Quaternion.Euler(0, shouldFlip ? 180 : 0, 0);
    }
    private void SetEnemyMoveRotation(EnemyData enemyData, Image enemyImage, string fromLocation, string toLocation)
    {
        int fromLocationX = BattleManager.Instance.ConvertNormalPos(fromLocation)[0];
        int toLocationX = BattleManager.Instance.ConvertNormalPos(toLocation)[0];
        bool shouldFlip = (toLocationX > fromLocationX) ? enemyData.ImageFlip : !enemyData.ImageFlip;
        enemyImage.transform.localRotation = Quaternion.Euler(0, shouldFlip ? 180 : 0, 0);
    }
    // 处理敌人移动
    private IEnumerator HandleEnemyMove(string location, EnemyData enemyData, Enemy enemy, Image enemyImage, Dictionary<string, EnemyData> enemyDict)
    {
        string playerLocation = BattleManager.Instance.CurrentPlayerLocation;
        if (enemy.InRange)
        {
            SetEnemyAttackRotation(enemyData, enemyImage, playerLocation);
            yield break;
        }
        BattleManager.CheckEmptyType checkEmptyType = BattleManager.CheckEmptyType.Move;
        BattleManager.ActionRangeType actionRangeType = BattleManager.ActionRangeType.Default;
        string minLocation = BattleManager.Instance.GetCloseLocation(location, playerLocation, enemyData.StepCount, checkEmptyType, actionRangeType);
        List<string> routeList = BattleManager.Instance.GetRoute(location, minLocation, checkEmptyType);
        for (int k = 0; k < routeList.Count; k++)
        {
            int childCount = BattleManager.Instance.GetCheckerboardPoint(routeList[k]);
            RectTransform emptyPlace = BattleManager.Instance.CheckerboardTrans.GetChild(childCount).GetComponent<RectTransform>();
            SetEnemyMoveRotation(enemyData, enemyImage, location, routeList[k]);
            enemyData.EnemyTrans.DOAnchorPos(emptyPlace.localPosition, 0.25f);  // 移动动画
            /* enemy.MyAnimator.SetBool("isRunning", true);
             enemy.MyAnimator.SetBool("isRunning", false);*/
            yield return new WaitForSeconds(0.25f);
        }
        SetEnemyAttackRotation(enemyData, enemyImage, minLocation);
        BattleManager.Instance.Replace(enemyDict, location, minLocation);
        BattleManager.Instance.RefreshCheckerboardList();
    }
    // 处理敌人攻击
    private IEnumerator HandleEnemyAttack(EnemyData enemyData, Enemy enemy, PlayerData playerData, int attackCount)
    {
        if (enemy.MySequence == null)
        {
            for (int i = 0; i < attackCount; i++)
            {
                bool isWaitAnimation = i == 0;
                string particlePath = enemyData.AttackParticleEffectPath;
                Vector3 playerPos = BattleManager.Instance.PlayerTrans.position;
                Vector3 destination = new Vector3(playerPos.x - 3, playerPos.y, -1);
                yield return BattleManager.Instance.SetParticleEffect(enemy.MyAnimator, enemy.transform.position, destination, particlePath, isWaitAnimation);
                if (enemy.InRange)
                {
                    BattleManager.Instance.TakeDamage(enemyData, playerData, enemy.CurrentAttackPower, BattleManager.Instance.CurrentPlayerLocation, 0);
                }
                yield return new WaitForSecondsRealtime(0.15f);
            }
        }
        else
        {
            yield return enemy.MySequence.Play().WaitForCompletion();
        }
    }

    // 应用敌人的效果
    private IEnumerator ApplyEffect(Enemy enemy, EnemyData enemyData, string location)
    {
        if (enemy.InRange || enemy.NoNeedCheckInRange)
        {
            string key;
            int value;
            if (enemy.TemporaryEffect != "")
            {
                string[] effects = enemy.TemporaryEffect.Split("=");
                key = effects[0];
                value = int.Parse(effects[1]);
            }
            else
            {
                key = enemyData.CurrentAttackOrderStrs.ElementAt(enemyData.CurrentAttackOrderIndex).Item1;
                value = enemyData.CurrentAttackOrderStrs.ElementAt(enemyData.CurrentAttackOrderIndex).Item2;
            }
            EffectFactory.Instance.CreateEffect(key).ApplyEffect(value, location, enemy.TargetLocation);
            BattleManager.Instance.ShowCharacterStatusClue(enemy.StatusClueTrans, EffectFactory.Instance.CreateEffect(key).SetTitleText(), 0);
        }
        yield return new WaitForSecondsRealtime(3);
    }

    // 更新攻击顺序
    private void UpdateAttackOrder(EnemyData enemyData, Enemy enemy)
    {
        if (enemy.MyActionType != Enemy.ActionType.Move)
        {
            List<(string, int)> attackOrder = enemyData.CurrentAttackOrderStrs;
            if (enemyData.CurrentAttackOrderIndex >= attackOrder.Count - 1)
            {
                enemyData.CurrentAttackOrderIndex = 0;
            }
            else
            {
                enemyData.CurrentAttackOrderIndex++;
            }
        }
        //Debug.Log(enemyData.CurrentAttackOrderIndex + "/" + (enemyData.AttackOrderStrs.Count - 1).ToString());
    }

    private void ChangeTurn()
    {
        if (BattleManager.Instance.MyBattleType != BattleManager.BattleType.Attack)
        {
            return;
        }
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Enemy);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }
    private void PlayerMove()
    {
        bool playerCantMove = BattleManager.Instance.PlayerMoveCount < BattleManager.Instance.PlayerOnceMoveConsume;
        bool notInAttack = BattleManager.Instance.MyBattleType != BattleManager.BattleType.Attack;
        bool containsCantMoveEffect = BattleManager.Instance.CurrentNegativeState.ContainsKey(nameof(CantMoveEffect));
        if (playerCantMove || notInAttack || containsCantMoveEffect)
        {
            if (containsCantMoveEffect || playerCantMove)
            {
                BattleManager.Instance.ShowCharacterStatusClue(BattleManager.Instance.CurrentPlayer.StatusClueTrans, "無法移動", 0);
            }
            return;
        }
        string playerLocation = BattleManager.Instance.CurrentPlayerLocation;
        EffectFactory.Instance.CreateEffect(nameof(MoveEffect)).ApplyEffect(BattleManager.Instance.PlayerMoveCount, playerLocation, playerLocation);
    }
    private void RemoveEnemy(string key)
    {
        EnemyData enemyData = (EnemyData)BattleManager.Instance.IdentifyCharacter(key);
        Enemy enemy = enemyData.EnemyTrans.GetComponent<Enemy>();
        Dictionary<string, EnemyData> currentEnemyList = BattleManager.Instance.CurrentEnemyList;
        if (enemyData.CurrentHealth <= 0 && !enemy.IsDeath)
        {
            enemy.MyAnimator.SetTrigger("isDeath");
            if (!enemy.IsSuspendedAnimation)
            {
                if (currentEnemyList.ContainsKey(key))
                {
                    currentEnemyList.Remove(key);
                }
                else
                {
                    BattleManager.Instance.CurrentMinionsList.Remove(key);
                }
                EventManager.Instance.ClearEvents(enemyData);
                Destroy(enemyData.EnemyTrans.gameObject, 1f);
            }
            enemy.MyCollider.enabled = !enemy.IsSuspendedAnimation;
            enemy.IsDeath = true;
            CheckEnemyInfo();
        }
        else
        {
            enemy.MyAnimator.SetTrigger("isHited");
        }
        BattleManager.Instance.RefreshCheckerboardList();
        if (currentEnemyList.Count == 0)
        {
            AudioManager.Instance.BGMAudio(0);
            enemyInfo.SetActive(false);
            currentEnemyList.Clear();
            BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Win);
            BattleManager.Instance.CurrentNegativeState.Clear();
            BattleManager.Instance.CurrentPositiveState.Clear();
            BattleManager.Instance.CurrentAbilityList.Clear();
            BattleManager.Instance.CurrentTerrainList.Clear();
            BattleManager.Instance.CurrentTrapList.Clear();
            BattleManager.Instance.CurrentOnceBattlePositiveList.Clear();
            BattleManager.Instance.CurrentMinionsList.Clear();
            RemoveAllTerrian();
        }
    }
    private void RemoveAllTerrian()
    {
        for (int i = trapGroupTrans.childCount - 1; i >= 0; i--)
        {
            Destroy(trapGroupTrans.GetChild(i).gameObject);
        }
        for (int i = terrainTrans.childCount - 1; i >= 0; i--)
        {
            Destroy(terrainTrans.GetChild(i).gameObject);
        }
    }

    private void RefreshPotionBag()
    {
        for (int i = 0; i < potionGroupTrans.childCount; i++)
        {
            potionGroupTrans.GetChild(i).gameObject.SetActive(false);
            //Destroy(potionGroupTrans.GetChild(i).gameObject);
        }
        for (int i = 0; i < DataManager.Instance.PotionBag.Count; i++)
        {
            int avoidClosure = i;
            PotionItem potionItem = Instantiate(potionPrefab, potionGroupTrans).GetComponent<PotionItem>();
            Potion potionData = DataManager.Instance.PotionBag[avoidClosure];
            Image potionImage = potionItem.GetComponent<Image>();
            Button potionButton = potionItem.GetComponent<Button>();
            potionImage.sprite = Resources.Load<Sprite>(potionData.ItemImagePath);
            potionItem.InfoTitleText.text = potionData.ItemName;
            potionItem.InfoDescriptionText.text = potionData.ItemInfo;
            potionButton.onClick.AddListener(() => UsePotion(potionData.ItemID, avoidClosure));
            potionItem.PriceText.enabled = false;
            UnityAction unityAction_1 = () =>
            {
                potionItem.InfoGameObject.SetActive(true);
                potionItem.PotionCanvas.overrideSorting = true;
            };
            UnityAction unityAction_2 = () =>
            {
                potionItem.InfoGameObject.SetActive(false);
                potionItem.PotionCanvas.overrideSorting = false;
            };
            BattleManager.Instance.SetEventTrigger(potionItem.PotionEventTrigger, unityAction_1, unityAction_2);
        }
    }
    private void UsePotion(int itemID, int bagID)
    {
        if (BattleManager.Instance.MyBattleType == BattleManager.BattleType.Enemy)
        {
            return;
        }
        potionClueMenu.gameObject.SetActive(true);
        Button yesButton = potionClueMenu.GetChild(0).GetComponent<Button>();
        Button noButton = potionClueMenu.GetChild(1).GetComponent<Button>();
        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(() => UsePotionEffect(itemID, bagID));
        noButton.onClick.AddListener(() => potionClueMenu.gameObject.SetActive(false));
    }
    private void UsePotionEffect(int potionID, int bagID)
    {
        string playerLocation = BattleManager.Instance.CurrentPlayerLocation;
        Transform statusClueTrans = BattleManager.Instance.CurrentPlayer.StatusClueTrans;
        for (int i = 0; i < DataManager.Instance.PotionList[potionID].ItemEffectList.Count; i++)
        {
            string effectName = DataManager.Instance.PotionList[potionID].ItemEffectList.ElementAt(i).Key;
            int value = DataManager.Instance.PotionList[potionID].ItemEffectList.ElementAt(i).Value;
            float waitTime = 0.5f * i;
            EffectFactory.Instance.CreateEffect(effectName).ApplyEffect(value, playerLocation, playerLocation);
            BattleManager.Instance.ShowCharacterStatusClue(statusClueTrans, EffectFactory.Instance.CreateEffect(effectName).SetTitleText(), waitTime);
        }
        DataManager.Instance.PotionBag.RemoveAt(bagID);
        potionClueMenu.gameObject.SetActive(false);
        RefreshPotionBag();
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }
    private void Speedup()
    {
        Time.timeScale = Time.timeScale == 1 ? 1.5f : 1;
    }
    private IEnumerator BattleInitial()
    {
        if (MapManager.Instance.ChapterCount == 3)
        {
            AudioManager.Instance.BGMAudio(3);
        }
        else if (MapManager.Instance.LevelCount == 14)
        {
            AudioManager.Instance.BGMAudio(2);
        }
        else
        {
            AudioManager.Instance.BGMAudio(1);
        }
        RefreshPotionBag();

        // 获取当前的敌人列表和地形列表
        var enemyList = BattleManager.Instance.CurrentEnemyList;
        var minionList = BattleManager.Instance.CurrentMinionsList;
        var terrainList = BattleManager.Instance.CurrentTerrainList;
        // 处理敌人列表
        for (int i = 0; i < enemyList.Count; i++)
        {
            string key = enemyList.ElementAt(i).Key;
            EnemyData enemyData = enemyList.ElementAt(i).Value;
            int checkerboardPoint = BattleManager.Instance.GetCheckerboardPoint(key);
            Enemy enemy = Instantiate(enemyPrefab, enemyTrans);
            RectTransform enemyRect = enemy.GetComponent<RectTransform>();
            enemyRect.anchoredPosition = checkerboardTrans.GetChild(checkerboardPoint).localPosition;
            enemy.EnemyID = enemyData.CharacterID;
            enemy.EnemyImage.sprite = Resources.Load<Sprite>(enemyData.EnemyImagePath);
            enemy.MyAnimator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(enemyData.EnemyAniPath);
            enemy.CurrentAttackPower = enemyData.MaxAttack;
            enemyData.EnemyTrans = enemyRect;
            enemyData.CurrentHealth = DataManager.Instance.EnemyList[enemy.EnemyID].MaxHealth;
            enemy.MyEnemyData = enemyData;

            yield return null;
        }
        for (int i = 0; i < minionList.Count; i++)
        {
            string key = minionList.ElementAt(i).Key;
            EnemyData enemyData = minionList.ElementAt(i).Value;
            int checkerboardPoint = BattleManager.Instance.GetCheckerboardPoint(key);
            Enemy enemy = Instantiate(enemyPrefab, enemyTrans);
            RectTransform enemyRect = enemy.GetComponent<RectTransform>();
            enemyRect.anchoredPosition = checkerboardTrans.GetChild(checkerboardPoint).localPosition;
            enemy.EnemyID = enemyData.CharacterID;
            enemy.EnemyImage.sprite = Resources.Load<Sprite>(enemyData.EnemyImagePath);
            enemy.MyAnimator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(enemyData.EnemyAniPath);
            enemy.CurrentAttackPower = enemyData.MaxAttack;
            enemyData.EnemyTrans = enemyRect;
            enemyData.CurrentHealth = DataManager.Instance.EnemyList[enemy.EnemyID].MaxHealth;
            enemy.MyEnemyData = enemyData;

            yield return null;
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
            yield return null;
        }
        CheckBattleInfo();
        roundTip.GetComponent<Image>().sprite = playerRound;
        UpdateSkillUI(skillStateGroupTrans, BattleManager.Instance.CurrentPlayerData.StartSkillList, negativeRPrefab);
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Player);
    }
    private void EventBattleInitial(params object[] args)
    {
        StartCoroutine(BattleInitial());
    }

    private void EventPlayerTurn(params object[] args)
    {
        changeTurnButton.onClick.RemoveAllListeners();
        playerMoveButton.onClick.RemoveAllListeners();
        speedupButton.onClick.RemoveAllListeners();
        speedupButton.onClick.AddListener(Speedup);
        changeTurnButton.onClick.AddListener(ChangeTurn);
        playerMoveButton.onClick.AddListener(PlayerMove);
        roundTip.GetComponent<Image>().sprite = playerRound;
        StartCoroutine(UIManager.Instance.FadeOutIn(roundTip, 0.5f, 1, false));
    }
    private void EventDrawCard(params object[] args)
    {
        CheckEnemyInfo();
    }
    private void EventEnemyTurn(params object[] args)
    {
        UIManager.Instance.ClearMoveClue(true);
        BattleManager.Instance.ClearAllEventTriggers();
        roundTip.GetComponent<Image>().sprite = enemyRound;
        enemyInfo.SetActive(false);
        StartCoroutine(UIManager.Instance.FadeOutIn(roundTip, 0.5f, 1, false));
        StartCoroutine(EnemyAttack());
    }
    private void EventMove(params object[] args)
    {
        EventManager.Instance.DispatchEvent(EventDefinition.eventAfterMove);
        // CheckBattleInfo();
    }
    private void EventAfterMove(params object[] args)
    {
        CheckEnemyInfo();
        //Debug.Log("after");
    }
    private void EventTakeDamage(params object[] args)
    {
        string defenderLocation = (string)args[2];
        if (BattleManager.Instance.CurrentEnemyList.ContainsKey(defenderLocation) || BattleManager.Instance.CurrentMinionsList.ContainsKey(defenderLocation))
        {
            CheckEnemyInfo();
            RefreshEnemyInfo(defenderLocation);
            RemoveEnemy(defenderLocation);
        }
        else
        {
            if (volume.profile.TryGet(out Vignette v) && v.intensity.value == 0)
            {
                float currentCount = 0.0f;
                Sequence sequence = DOTween.Sequence();
                sequence.Append(DOTween.To(() => currentCount, x =>
                {
                    currentCount = x;
                    v.intensity.Override(currentCount);
                    bloodScreenCanvasGroup.alpha = currentCount;
                }, maxVignetteIntensity, firstBloodScreenDuration).SetEase(Ease.OutQuad));
                sequence.Append(DOTween.To(() => currentCount, x =>
                {
                    currentCount = x;
                    v.intensity.Override(currentCount);
                    bloodScreenCanvasGroup.alpha = currentCount;
                }, 0f, secondBloodScreenDuration).SetEase(Ease.InQuad));
                sequence.Play();
            }
        }
        EventRecover(args);
    }
    private void EventRecover(params object[] args)
    {
        if ((int)args[1] < 0)
        {
            return;
        }
        GameObject damageNum = Instantiate(damageNumPrefab, UI.transform);
        RectTransform damageRect = damageNum.GetComponent<RectTransform>();
        Text damageText = damageNum.GetComponentInChildren<Text>();
        string damageStr = args.Length > 6 ? args[6].ToString() : args[1].ToString();
        Color color = (Color)args[3];
        if (damageStr == "0" && args.Length > 6)
        {
            if (BattleManager.Instance.CurrentOnceBattlePositiveList.ContainsKey("DamageImmunityEffect"))
            {
                BattleManager.Instance.AddState(BattleManager.Instance.CurrentOnceBattlePositiveList, "DamageImmunityEffect", -1);
                damageStr = "免傷";

            }
            else if (((CharacterData)args[5]).DamageReduction == 100)
            {
                damageStr = "免疫";
            }
            else
            {
                damageStr = "格擋";
            }
            color = Color.white;
        }
        damageText.text = damageStr;
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
        var usedCardBag = DataManager.Instance.UsedCardBag;
        var removeCardBag = DataManager.Instance.RemoveCardBag;
        var negativeState = BattleManager.Instance.CurrentNegativeState;
        var positiveList = BattleManager.Instance.CurrentPositiveState;
        var oncePositiveList = BattleManager.Instance.CurrentOnceBattlePositiveList;
        // 更新UI文本
        actionPointText.text = $"{playerData.CurrentActionPoint}/{playerData.MaxActionPoint}";
        manaPointText.text = playerData.Mana.ToString();
        /* health.DOFillAmount((float)playerData.CurrentHealth / playerData.MaxHealth, 0.5f);
         healthText.text = $"{playerData.CurrentHealth}/{playerData.MaxHealth}";*/
        shieldText.text = playerData.CurrentShield.ToString();
        usedCardBagCountText.text = usedCardBag.Count.ToString();
        removeCardBagCountText.text = removeCardBag.Count.ToString();

        // 更新玩家移动网格
        int playerMoveCount = BattleManager.Instance.PlayerMoveCount;
        for (int i = 0; i < playerMoveGridTrans.childCount; i++)
        {
            playerMoveGridTrans.GetChild(i).gameObject.SetActive(i < playerMoveCount);
        }
        UpdateStateUI(negativeGroupTrans, negativeState, negativeRPrefab, false, true);
        UpdateStateUI(positiveGroupTrans, positiveList, negativeRPrefab, false, true);
        UpdateStateUI(positiveGroupTrans, oncePositiveList, negativeRPrefab, false, false);
    }

    private void UpdateStateUI(Transform groupTrans, Dictionary<string, int> stateList, BattleState prefab, bool isPassiveEffect, bool canDestroy)
    {
        // 清空当前状态
        if (canDestroy)
        {
            for (int i = groupTrans.childCount - 1; i >= 0; i--)
            {
                Destroy(groupTrans.GetChild(i).gameObject);
            }
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
            IEffect effect = EffectFactory.Instance.CreateEffect(key);
            Sprite effectSprite = effect.SetIcon();
            if (effectSprite == null)
            {
                effectSprite = EffectFactory.Instance.CreateEffect("KnockBackEffect").SetIcon();
            }
            EventTrigger eventTrigger = infoGroupTrans.GetComponent<EventTrigger>();
            UnityAction unityAction_1 = () => { infoGroupTrans.GetChild(0).gameObject.SetActive(true); };
            UnityAction unityAction_2 = () => { infoGroupTrans.GetChild(0).gameObject.SetActive(false); };
            BattleManager.Instance.SetEventTrigger(eventTrigger, unityAction_1, unityAction_2);
            if (!isPassiveEffect)
            {
                stateText.text = value.ToString();
            }
            stateObject.DisableImage.SetActive(value == -1);
            stateImage.sprite = effectSprite;
            infoTitle.text = effect.SetTitleText();
            string descriptionStr = isPassiveEffect ? effect.SetPassiveEffectDescriptionText() : effect.SetDescriptionText();
            if (descriptionStr.Length > 30)
            {
                infoGroupTrans.GetChild(0).GetChild(0).gameObject.SetActive(true);
                descriptionStr = BattleManager.Instance.AutomaticLineWrapping(descriptionStr, 30);
            }
            infoDescription.text = descriptionStr;
        }
    }
    private void UpdateSkillUI(Transform groupTrans, List<int> stateList, BattleState prefab)
    {
        for (int i = groupTrans.childCount - 1; i >= 0; i--)
        {
            Destroy(groupTrans.GetChild(i).gameObject);
        }
        for (int i = 0; i < stateList.Count; i++)
        {
            Skill skill = DataManager.Instance.SkillList[stateList[i]];
            BattleState stateObject = Instantiate(prefab, groupTrans);
            Image stateImage = stateObject.BattleStateImage;
            Text stateText = stateObject.BattleStateAmount;
            Text infoTitle = stateObject.InfoTitle;
            Text infoDescription = stateObject.InfoDescription;
            Transform infoGroupTrans = stateObject.InfoGroupTrans;
            Sprite effectSprite = skill.SkillSprite;
            if (effectSprite == null)
            {
                effectSprite = EffectFactory.Instance.CreateEffect("KnockBackEffect").SetIcon();
            }
            EventTrigger eventTrigger = infoGroupTrans.GetComponent<EventTrigger>();
            UnityAction unityAction_1 = () => { infoGroupTrans.GetChild(0).gameObject.SetActive(true); };
            UnityAction unityAction_2 = () => { infoGroupTrans.GetChild(0).gameObject.SetActive(false); };
            BattleManager.Instance.SetEventTrigger(eventTrigger, unityAction_1, unityAction_2);
            //stateObject.DisableImage.SetActive(value == -1);
            stateImage.sprite = effectSprite;
            infoTitle.text = skill.SkillName;
            string descriptionStr = skill.SkillDescrption;
            if (descriptionStr.Length > 30)
            {
                infoGroupTrans.GetChild(0).GetChild(0).gameObject.SetActive(true);
                descriptionStr = BattleManager.Instance.AutomaticLineWrapping(descriptionStr, 30);
            }
            infoDescription.text = descriptionStr;
        }
    }
    private void EventNextChapter(params object[] args)
    {
        battleBackground.sprite = battleBackgroundSpriteList[MapManager.Instance.ChapterCount - 1];
        // UI.SetActive(true);
    }
}