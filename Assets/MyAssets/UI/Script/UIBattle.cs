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
    private Text cardBagCountText;

    [SerializeField]
    private Text usedCardBagCountText;
    [SerializeField]
    private Text removeCardBagCountText;

    [SerializeField]
    private Button changeTurnButton;

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
    private Text enemyLocation;
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
        BattleManager.Instance.CheckerboardTrans = checkerboardTrans;
        EventManager.Instance.AddEventRegister(EventDefinition.eventRefreshUI, EventRefreshUI);
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
        EventManager.Instance.AddEventRegister(EventDefinition.eventBattleInitial, EventBattleInitial);
        EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
        EventManager.Instance.AddEventRegister(EventDefinition.eventEnemyTurn, EventEnemyTurn);
        //Hide();
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        yield return null;
        Hide();
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
        float distance = BattleManager.Instance.GetDistance(location);
        bool checkTerrainObstacles = BattleManager.Instance.CheckTerrainObstacles(location, BattleManager.Instance.CurrentEnemyList[location].AlertDistance
          , BattleManager.Instance.CurrentLocationID, BattleManager.CheckEmptyType.EnemyAttack);
        if (distance <= BattleManager.Instance.CurrentEnemyList[location].AttackDistance && !checkTerrainObstacles)
            UIManager.Instance.ChangeCheckerboardColor(Color.red, location, BattleManager.Instance.CurrentEnemyList[location].AttackDistance, BattleManager.CheckEmptyType.EnemyAttack);
        else
            UIManager.Instance.ChangeCheckerboardColor(Color.yellow, location, BattleManager.Instance.CurrentEnemyList[location].StepCount, BattleManager.CheckEmptyType.EnemyAttack);
        int x = BattleManager.Instance.ConvertNormalPos(location)[0];
        int y = BattleManager.Instance.ConvertNormalPos(location)[1];
        //enemyInfo.SetActive(true);
        enemyName.text = BattleManager.Instance.CurrentEnemyList[location].CharacterName;
        enemyLocation.text = x.ToString() + "，" + y.ToString();
        enemyImage.sprite = Resources.Load<Sprite>(BattleManager.Instance.CurrentEnemyList[location].EnemyImagePath);
        enemyHealth.text = BattleManager.Instance.CurrentEnemyList[location].CurrentHealth.ToString()
        + "/" + BattleManager.Instance.CurrentEnemyList[location].MaxHealth.ToString();
        enemyShield.text = BattleManager.Instance.CurrentEnemyList[location].CurrentShield.ToString();
    }

    public void ChangeTurn()
    {
        if (BattleManager.Instance.MyBattleType != BattleManager.BattleType.Attack)
            return;
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Enemy);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }

    private void RemoveEnemy(string key)
    {
        if (BattleManager.Instance.CurrentEnemyList[key].CurrentHealth <= 0)
        {
            Destroy(enemyTrans.GetChild(BattleManager.Instance.CurrentEnemyList.Values.ToList().IndexOf(BattleManager.Instance.CurrentEnemyList[key])).gameObject);
            BattleManager.Instance.CurrentEnemyList.Remove(key);
        }

        if (BattleManager.Instance.CurrentEnemyList.Count == 0)
            BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Win);
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
            Enemy enemy = Instantiate(enemyPrefab, enemyTrans);
            enemy.EnemyLocation = key;
            enemy.GetComponent<RectTransform>().anchoredPosition = BattleManager.Instance.CheckerboardTrans
            .GetChild(BattleManager.Instance.GetCheckerboardPoint(key)).localPosition;
            enemy.EnemyID = BattleManager.Instance.CurrentEnemyList[key].CharacterID;
            enemy.EnemyImage.sprite = Resources.Load<Sprite>(BattleManager.Instance.CurrentEnemyList[key].EnemyImagePath);
            BattleManager.Instance.CurrentEnemyList[key].EnemyTrans = enemy.GetComponent<RectTransform>();
            BattleManager.Instance.CurrentEnemyList[key].CurrentHealth = DataManager
                .Instance
                .EnemyList[enemy.EnemyID].MaxHealth;
            yield return null;
        }
        for (int i = 0; i < BattleManager.Instance.CurrentTerrainList.Count; i++)
        {
            string key = BattleManager.Instance.CurrentTerrainList.ElementAt(i).Key;
            GameObject terrain = Instantiate(terrainPrefab, terrainTrans);
            terrain.GetComponent<RectTransform>().anchoredPosition = BattleManager.Instance.CheckerboardTrans
            .GetChild(BattleManager.Instance.GetCheckerboardPoint(key)).localPosition;
            /*terrain.ImagePath.sprite = Resources.Load<Sprite>(BattleManager.Instance.CurrentEnemyList[key].EnemyImagePath);
            BattleManager.Instance.CurrentEnemyList[key].EnemyTrans = enemy.GetComponent<RectTransform>();
            BattleManager.Instance.CurrentEnemyList[key].CurrentHealth = DataManager
                .Instance
                .EnemyList[enemy.EnemyID].MaxHealth;*/
            yield return null;
        }
        StartCoroutine(UIManager.Instance.RefreshEnemyAlert());
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
    }
    private void EventEnemyTurn(params object[] args)
    {
        UIManager.Instance.ClearMoveClue(true);
        ClearAllEventTriggers();
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
        cardBagCountText.text = DataManager.Instance.CardBag.Count.ToString();
        usedCardBagCountText.text = DataManager.Instance.UsedCardBag.Count.ToString();
        removeCardBagCountText.text = DataManager.Instance.RemoveCardBag.Count.ToString();
    }
}
