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
    private Text cardBagCountText;

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
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        yield return null;
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
        Hide();
    }
    private void CheckBattleInfo()
    {
        for (int i = 0; i < battleInfoList.Count; i++)
        {
            int id = i;
            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;
            entryExit.eventID = EventTriggerType.PointerExit;
            entryEnter.callback.AddListener((arg) => { battleInfoList[id].transform.GetChild(0).gameObject.SetActive(true); });
            entryExit.callback.AddListener((arg) => { battleInfoList[id].transform.GetChild(0).gameObject.SetActive(false); });
            battleInfoList[id].triggers.Add(entryEnter);
            battleInfoList[id].triggers.Add(entryExit);
        }
        BattleManager.Instance.PlayerMoveCount++;
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
            UIManager.Instance.ChangeCheckerboardColor(Color.red, location, enemyData.AttackDistance, BattleManager.CheckEmptyType.EnemyAttack);
        else
            UIManager.Instance.ChangeCheckerboardColor(Color.yellow, location, enemyData.StepCount, BattleManager.CheckEmptyType.EnemyAttack);
        enemyInfo.SetActive(true);
        enemyName.text = enemyData.CharacterName;
        enemyImage.sprite = Resources.Load<Sprite>(enemyData.EnemyImagePath);
        enemyHealth.text = enemyData.CurrentHealth.ToString() + "/" + enemyData.MaxHealth.ToString();
        enemyShield.text = enemyData.CurrentShield.ToString();
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
        if (BattleManager.Instance.PlayerMoveCount <= 0 || BattleManager.Instance.MyBattleType != BattleManager.BattleType.Attack
        || BattleManager.Instance.CurrentNegativeState.Contains(BattleManager.NegativeState.CantMove))
            return;
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.UsingEffect);
        EffectFactory.Instance.CreateEffect("MoveEffect").ApplyEffect(BattleManager.Instance.PlayerMoveCount, "Player");
    }
    private void RemoveEnemy(string key)
    {
        EnemyData enemyData = BattleManager.Instance.CurrentEnemyList[key];
        if (enemyData.CurrentHealth <= 0)
        {
            enemyData.EnemyTrans.GetComponent<Enemy>().MyAnimator.SetTrigger("isDeath");
            Destroy(enemyData.EnemyTrans.gameObject, 1);
            BattleManager.Instance.CurrentEnemyList.Remove(key);
            ClearAllEventTriggers();
            CheckEnemyInfo();
            CheckBattleInfo();
        }
        enemyData.EnemyTrans.GetComponent<Enemy>().MyAnimator.SetTrigger("isHited");
        BattleManager.Instance.RefreshCheckerboardList();
        if (BattleManager.Instance.CurrentEnemyList.Count == 0)
        {
            BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Win);
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
        StartCoroutine(BattleInitial());
    }

    private IEnumerator BattleInitial()
    {
        RefreshPotionBag();
        for (int i = 0; i < BattleManager.Instance.CurrentEnemyList.Count; i++)
        {
            string key = BattleManager.Instance.CurrentEnemyList.ElementAt(i).Key;
            int checkerboardPoint = BattleManager.Instance.GetCheckerboardPoint(key);
            Enemy enemy = Instantiate(enemyPrefab, enemyTrans);
            enemy.EnemyLocation = key;
            enemy.GetComponent<RectTransform>().anchoredPosition = BattleManager.Instance.CheckerboardTrans.GetChild(checkerboardPoint).localPosition;
            enemy.EnemyID = BattleManager.Instance.CurrentEnemyList[key].CharacterID;
            enemy.EnemyImage.sprite = Resources.Load<Sprite>(BattleManager.Instance.CurrentEnemyList[key].EnemyImagePath);
            enemy.MyAnimator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(BattleManager.Instance.CurrentEnemyList[key].EnemyAniPath);
            BattleManager.Instance.CurrentEnemyList[key].EnemyTrans = enemy.GetComponent<RectTransform>();
            BattleManager.Instance.CurrentEnemyList[key].CurrentHealth = DataManager.Instance.EnemyList[enemy.EnemyID].MaxHealth;
            yield return null;
        }
        for (int i = 0; i < BattleManager.Instance.CurrentTerrainList.Count; i++)
        {
            string key = BattleManager.Instance.CurrentTerrainList.ElementAt(i).Key;
            GameObject terrain = Instantiate(terrainPrefab, terrainTrans);
            RectTransform terrainRect = terrain.GetComponent<RectTransform>();
            terrainRect.anchoredPosition = BattleManager.Instance.CheckerboardTrans.GetChild(BattleManager.Instance.GetCheckerboardPoint(key)).localPosition;
            yield return null;
        }
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
        EffectFactory.Instance.CreateEffect(effectName).ApplyEffect(value, "Player");
        DataManager.Instance.PotionBag.RemoveAt(bagID);
        potionClueMenu.gameObject.SetActive(false);
        RefreshPotionBag();
        EventRefreshUI();
    }
    private void EventPlayerTurn(params object[] args)
    {
        CheckEnemyInfo();
        CheckBattleInfo();
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
        CheckBattleInfo();
    }
    private void EventTakeDamage(params object[] args)
    {
        if (BattleManager.Instance.CurrentLocationID != (string)args[2])
            RemoveEnemy((string)args[2]);
        GameObject damageNum = Instantiate(damageNumPrefab, UI.transform);
        RectTransform damageRect = damageNum.GetComponent<RectTransform>();
        Text damageText = damageNum.GetComponentInChildren<Text>();
        damageText.text = args[1].ToString();
        damageText.DOColor(new Color(60f / 255f, 0, 0), 0.75f);
        int direction = Random.Range(0, 2);
        if (direction == 0)
            xOffset *= -1;
        Vector2 startPoint = (Vector2)args[0];
        Vector2 endPoint = new(startPoint.x + xOffset, -540);
        Vector2 midPoint = new(startPoint.x + xOffset / 2, startPoint.y + curveHeight);
        Vector2 endScale = new(0.5f, 0.5f);
        DOTween
            .To(
                (t) =>
                {
                    Vector2 position = UIManager.Instance.GetBezierCurve(
                        startPoint,
                        midPoint,
                        endPoint,
                        t
                    );
                    damageRect.anchoredPosition = position;
                },
                0,
                1,
                1
            )
            .SetEase(Ease.OutQuad);
        damageRect.DOScale(endScale, 1);
        StartCoroutine(UIManager.Instance.FadeOutIn(damageNum.GetComponent<CanvasGroup>(), 1f, 0, true));
    }

    private void EventRefreshUI(params object[] args)
    {
        int id = DataManager.Instance.PlayerID;
        actionPointText.text = DataManager.Instance.PlayerList[id].CurrentActionPoint.ToString() + "/" + DataManager.Instance.PlayerList[id].MaxActionPoint.ToString();
        manaPointText.text = DataManager.Instance.PlayerList[id].Mana.ToString();
        health.DOFillAmount((float)((float)DataManager.Instance.PlayerList[id].CurrentHealth / DataManager.Instance.PlayerList[id].MaxHealth), 0.5f);
        healthText.text = DataManager.Instance.PlayerList[id].CurrentHealth.ToString() + "/" + DataManager.Instance.PlayerList[id].MaxHealth.ToString();
        shieldText.text = DataManager.Instance.PlayerList[id].CurrentShield.ToString();
        battleCardBagCountText.text = DataManager.Instance.CardBag.Count.ToString();
        cardBagCountText.text = DataManager.Instance.CardBag.Count.ToString();
        usedCardBagCountText.text = DataManager.Instance.UsedCardBag.Count.ToString();
        removeCardBagCountText.text = DataManager.Instance.RemoveCardBag.Count.ToString();
        for (int i = 0; i < playerMoveGridTrans.childCount; i++)
        {
            playerMoveGridTrans.GetChild(i).gameObject.SetActive(false);
        }
        for (int i = 0; i < BattleManager.Instance.PlayerMoveCount; i++)
        {
            playerMoveGridTrans.GetChild(i).gameObject.SetActive(true);
        }
    }
}
