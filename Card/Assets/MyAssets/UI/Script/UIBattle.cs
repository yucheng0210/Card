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
    private int currentHealth;

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
        currentHealth = BattleManager.Instance.PlayerList[0].MaxHealth;
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Player);
        UIManager.Instance.ShowHealthUI(
            BattleManager.Instance.PlayerList[0].MaxHealth,
            currentHealth
        );
    }
}
