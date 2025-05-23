using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Character
{
    public int AdditionPower { get; set; }
    private void Start()
    {
        BattleManager.Instance.CurrentPlayer = this;
        BattleManager.Instance.PlayerTrans = GetComponent<RectTransform>();
        BattleManager.Instance.PlayerAni = GetComponent<Animator>();
        BattleManager.Instance.CurrentPlayerData.CurrentHealth = BattleManager.Instance.CurrentPlayerData.MaxHealth;
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
        EventManager.Instance.AddEventRegister(EventDefinition.eventBattleInitial, EventBattleInitial);
    }
    private void EventTakeDamage(params object[] args)
    {
        if (BattleManager.Instance.CurrentPlayerData.CurrentHealth <= 0)
        {
            EventManager.Instance.DispatchEvent(EventDefinition.eventGameOver, false);
            EventManager.Instance.DispatchEvent(EventDefinition.eventReloadGame);
            Destroy(gameObject);
        }
    }
    private void EventBattleInitial(params object[] args)
    {
        AdditionPower = 0;
    }
}
