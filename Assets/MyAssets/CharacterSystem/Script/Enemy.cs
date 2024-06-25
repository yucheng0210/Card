using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private Image alertImage;
    [SerializeField]
    private Slider enemyHealthSlider;

    [SerializeField]
    private Text enemyHealthText;

    [SerializeField]
    private Text enemyAttackIntentText;

    public Image EnemyAlert
    {
        get { return alertImage; }
        set { alertImage = value; }
    }
    public Image EnemyImage { get; set; }
    public int EnemyID { get; set; }
    public string EnemyLocation { get; set; }

    public enum AttackType
    {
        Attack,
        Shield,
        Effect
    }

    private void Awake()
    {
        EnemyImage = GetComponent<Image>();
        EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
    }
    private void OnDisable()
    {
        EventManager.Instance.RemoveEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
    }
    private void EventPlayerTurn(params object[] args)
    {
        int randomAttack = UnityEngine.Random.Range(DataManager.Instance.EnemyList[EnemyID].MinAttack, DataManager.Instance.EnemyList[EnemyID].MaxAttack + 1);
        BattleManager.Instance.CurrentEnemyList[EnemyLocation].CurrentAttack = randomAttack;
        enemyAttackIntentText.text = randomAttack.ToString();
    }
}
