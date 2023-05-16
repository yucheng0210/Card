using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        buttonText.text = "敵方回合";
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Enemy);
    }

    public void EventRefreshUI(params object[] args)
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
