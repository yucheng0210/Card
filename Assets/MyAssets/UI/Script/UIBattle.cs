using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

public class UIBattle : UIBase
{
    [Header("玩家")]
    [SerializeField]
    private RectTransform playerHealthRect;

    [SerializeField]
    private RectTransform playerHurtRect;

    [SerializeField]
    private Text actionPointText;

    [SerializeField]
    private Text manaPointText;

    [SerializeField]
    private Text shieldText;

    [SerializeField]
    private Slider healthSlider;

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
    [SerializeField]
    private RectTransform enemyHealthRect;

    [SerializeField]
    private RectTransform enemyHurtRect;

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
    public Text ActionPointText
    {
        get { return actionPointText; }
        set { actionPointText = value; }
    }
    public Text ShieldText
    {
        get { return shieldText; }
        set { shieldText = value; }
    }
    public Slider HealthSlider
    {
        get { return healthSlider; }
        set { healthSlider = value; }
    }
    public Text HealthText
    {
        get { return healthText; }
        set { healthText = value; }
    }

    protected override void Start()
    {
        base.Start();
        changeTurnButton.onClick.AddListener(ChangeTurn);
        BattleManager.Instance.CheckerboardTrans = checkerboardTrans;
        EventManager.Instance.AddEventRegister(EventDefinition.eventRefreshUI, EventRefreshUI);
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
        EventManager.Instance.AddEventRegister(
            EventDefinition.eventBattleInitial,
            EventBattleInitial
        );
        EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
        EventManager.Instance.AddEventRegister(EventDefinition.eventEnemyTurn, EventEnemyTurn);
        //Hide();
        StartCoroutine(StartGame());
    }

    private void Update()
    {
        UpdateValue();
    }
    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(0.5f);
        Hide();
    }
    private void CheckEnemyInfo()
    {
        for (int i = 0; i < checkerboardTrans.childCount; i++)
        {
            string location = BattleManager.Instance.ConvertCheckerboardPos(i);
            checkerboardTrans.GetChild(i).GetComponent<Button>().onClick.AddListener(() => RefreshEnemyInfo(location));
        }
    }
    private void RefreshEnemyInfo(string location)
    {
        UIManager.Instance.ClearMoveClue(false);
        if (!BattleManager.Instance.CurrentEnemyList.ContainsKey(location))
            return;
        UIManager.Instance.ChangeCheckerboardColor
        (Color.yellow, location, BattleManager.Instance.CurrentEnemyList[location].AlertDistance, BattleManager.CheckEmptyType.EnemyAttack);
        UIManager.Instance.ChangeCheckerboardColor
        (Color.red, location, BattleManager.Instance.CurrentEnemyList[location].StepCount, BattleManager.CheckEmptyType.EnemyAttack);
        int x = BattleManager.Instance.ConvertNormalPos(location)[0];
        int y = BattleManager.Instance.ConvertNormalPos(location)[1];
        enemyInfo.SetActive(true);
        enemyName.text = BattleManager.Instance.CurrentEnemyList[location].CharacterName;
        enemyLocation.text = x.ToString() + "，" + y.ToString();
        enemyImage.sprite = Resources.Load<Sprite>(BattleManager.Instance.CurrentEnemyList[location].EnemyImagePath);
        enemyHealth.text = BattleManager.Instance.CurrentEnemyList[location].CurrentHealth.ToString()
        + "/" + BattleManager.Instance.CurrentEnemyList[location].MaxHealth.ToString();
        enemyShield.text = BattleManager.Instance.CurrentEnemyList[location].CurrentShield.ToString();
    }

    private void UpdateValue()
    {
        if (playerHurtRect == null || enemyHealthRect == null)
            return;
        enemyHurtRect.anchorMax = new Vector2(
            Mathf.Lerp(enemyHurtRect.anchorMax.x, enemyHealthRect.anchorMax.x, Time.deltaTime * 5),
            enemyHurtRect.anchorMax.y
        );
        playerHurtRect.anchorMax = new Vector2(
            Mathf.Lerp(
                playerHurtRect.anchorMax.x,
                playerHealthRect.anchorMax.x,
                Time.deltaTime * 5
            ),
            playerHurtRect.anchorMax.y
        );
    }

    public void ChangeTurn()
    {
        if (BattleManager.Instance.MyBattleType != BattleManager.BattleType.Attack)
            return;
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Enemy);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }

    private IEnumerator RemoveEnemy()
    {
        for (int i = BattleManager.Instance.CurrentEnemyList.Count - 1; i >= 0; i--)
        {
            string key = BattleManager.Instance.CurrentEnemyList.ElementAt(i).Key;
            if (BattleManager.Instance.CurrentEnemyList[key].CurrentHealth <= 0)
            {
                Destroy(enemyTrans.GetChild(i).gameObject);
                BattleManager.Instance.CurrentEnemyList.Remove(key);
            }
            yield return null;
        }
        /*for (int i = 0; i < enemyTrans.childCount; i++)
        {
            enemyTrans.GetChild(i).GetComponent<Enemy>().EnemyLocation = i;
        }*/
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
    }
    private void EventPlayerTurn(params object[] args)
    {
        CheckEnemyInfo();
        Text buttonText = changeTurnButton.GetComponentInChildren<Text>();
        //buttonText.text = "結束回合";
    }
    private void EventEnemyTurn(params object[] args)
    {
        UIManager.Instance.ClearMoveClue(true);
    }
    private void EventTakeDamage(params object[] args)
    {
        StartCoroutine(RemoveEnemy());
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
        StartCoroutine(
            UIManager.Instance.FadeOutIn(damageNum.GetComponent<CanvasGroup>(), 1f, 0, true)
        );
    }

    private void EventRefreshUI(params object[] args)
    {
        int id = DataManager.Instance.PlayerID;
        actionPointText.text =
            DataManager.Instance.PlayerList[id].CurrentActionPoint.ToString()
            + "/"
            + DataManager.Instance.PlayerList[id].MaxActionPoint.ToString();
        manaPointText.text = DataManager.Instance.PlayerList[id].Mana.ToString();
        healthSlider.value = (float)(
            (float)DataManager.Instance.PlayerList[id].CurrentHealth
            / DataManager.Instance.PlayerList[id].MaxHealth
        );
        healthText.text =
            DataManager.Instance.PlayerList[id].CurrentHealth.ToString()
            + "/"
            + DataManager.Instance.PlayerList[id].MaxHealth.ToString();
        shieldText.text = DataManager.Instance.PlayerList[id].CurrentShield.ToString();
        cardBagCountText.text = DataManager.Instance.CardBag.Count.ToString();
        usedCardBagCountText.text = DataManager.Instance.UsedCardBag.Count.ToString();
        removeCardBagCountText.text = DataManager.Instance.RemoveCardBag.Count.ToString();
    }
}
