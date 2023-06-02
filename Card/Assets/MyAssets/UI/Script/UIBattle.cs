using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIBattle : UIBase
{
    [SerializeField]
    private Text actionPointText;

    [SerializeField]
    private Text shieldText;

    [SerializeField]
    private Slider healthSlider;

    [SerializeField]
    private Text healthText;

    [SerializeField]
    private Slider enemyHealthSlider;

    [SerializeField]
    private Text enemyHealthText;

    [SerializeField]
    private RectTransform enemyHealthRect;

    [SerializeField]
    private RectTransform enemyHurtRect;

    [SerializeField]
    private Button changeTurnButton;

    [Header("傷害特效")]
    [SerializeField]
    private GameObject damageNumPrefab;

    [SerializeField]
    private GameObject hitPrefab;

    [SerializeField]
    private float xOffset;

    [SerializeField]
    private float curveHeight;

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

    private void Start()
    {
        changeTurnButton.onClick.AddListener(ChangeTurn);
        EventManager.Instance.AddEventRegister(EventDefinition.eventRefreshUI, EventRefreshUI);
        EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
    }

    private void Update()
    {
        enemyHurtRect.anchorMax = new Vector2(
            Mathf.Lerp(enemyHurtRect.anchorMax.x, enemyHealthRect.anchorMax.x, Time.deltaTime * 5),
            enemyHurtRect.anchorMax.y
        );
    }

    public void ShowEnemyHealth(int maxHealth, int currentHealth)
    {
        enemyHealthSlider.value = (currentHealth / (float)maxHealth);
        enemyHealthText.text = currentHealth.ToString() + "/" + maxHealth.ToString();
    }

    public void ChangeTurn()
    {
        Text buttonText = changeTurnButton.GetComponentInChildren<Text>();
        if (BattleManager.Instance.MyBattleType == BattleManager.BattleType.Player)
        {
            buttonText.text = "敵方回合";
            BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Enemy);
        }
    }

    private void EventTakeDamage(params object[] args)
    {
        GameObject damageNum = Instantiate(damageNumPrefab, UI.transform);
        RectTransform damageRect = damageNum.GetComponent<RectTransform>();
        Text damageText = damageNum.GetComponentInChildren<Text>();
        damageText.text = args[1].ToString();
        int direction = UnityEngine.Random.Range(0, 2);
        if (direction == 0)
            xOffset *= -1;
        Vector2 startPoint = (Vector2)args[0];
        Vector2 endPoint = new Vector2(startPoint.x + xOffset, -540);
        Vector2 midPoint = new Vector2(startPoint.x + xOffset / 2, startPoint.y + curveHeight);
        Vector2 endScale = new Vector2(0.5f, 0.5f);
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

    private void EventPlayerTurn(params object[] args)
    {
        Text buttonText = changeTurnButton.GetComponentInChildren<Text>();
        buttonText.text = "結束回合";
    }

    private void EventRefreshUI(params object[] args)
    {
        actionPointText.text =
            DataManager.Instance.PlayerList[
                DataManager.Instance.PlayerID
            ].CurrentActionPoint.ToString()
            + "/"
            + DataManager.Instance.PlayerList[
                DataManager.Instance.PlayerID
            ].MaxActionPoint.ToString();
        healthSlider.value = (float)(
            (float)DataManager.Instance.PlayerList[DataManager.Instance.PlayerID].CurrentHealth
            / DataManager.Instance.PlayerList[DataManager.Instance.PlayerID].MaxHealth
        );
        healthText.text =
            DataManager.Instance.PlayerList[DataManager.Instance.PlayerID].CurrentHealth.ToString()
            + "/"
            + DataManager.Instance.PlayerList[DataManager.Instance.PlayerID].MaxHealth.ToString();
        enemyHealthSlider.value = (float)(
            (float)DataManager.Instance.EnemyList[1001].CurrentHealth
            / DataManager.Instance.EnemyList[1001].MaxHealth
        );
        enemyHealthText.text =
            DataManager.Instance.EnemyList[1001].CurrentHealth.ToString()
            + "/"
            + DataManager.Instance.EnemyList[1001].MaxHealth.ToString();
        shieldText.text = DataManager.Instance.PlayerList[
            DataManager.Instance.PlayerID
        ].CurrentShield.ToString();
    }
}
