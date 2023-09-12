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
    private Button changeTurnButton;

    [Header("敵人")]
    [SerializeField]
    private Enemy enemyPrefab;

    [SerializeField]
    private Transform enemyTrans;

    [SerializeField]
    private Vector2 enemyDistance;

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
    }

    private void Update()
    {
        UpdateValue();
    }

    private void UpdateValue()
    {
        if (playerHurtRect == null)
            return;
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
        Text buttonText = changeTurnButton.GetComponentInChildren<Text>();
        buttonText.text = "敵方回合";
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
        StartCoroutine(CreateEnemyAndTerrain());
    }

    private IEnumerator CreateEnemyAndTerrain()
    {
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
        BattleManager.Instance.RefreshEnemyAlert();
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Player);
    }

    private void EventPlayerTurn(params object[] args)
    {
        Text buttonText = changeTurnButton.GetComponentInChildren<Text>();
        buttonText.text = "結束回合";
    }

    private void EventTakeDamage(params object[] args)
    {
        StartCoroutine(RemoveEnemy());
        GameObject damageNum = Instantiate(damageNumPrefab, UI.transform);
        RectTransform damageRect = damageNum.GetComponent<RectTransform>();
        Text damageText = damageNum.GetComponentInChildren<Text>();
        damageText.text = args[1].ToString();
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
    }
}
