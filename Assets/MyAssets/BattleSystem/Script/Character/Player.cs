using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField]
    private Transform statusClueTrans;
    public Transform StatusClueTrans { get { return statusClueTrans; } set { statusClueTrans = value; } }
    private void Start()
    {
        BattleManager.Instance.CurrentPlayer = this;
        BattleManager.Instance.PlayerTrans = GetComponent<RectTransform>();
        BattleManager.Instance.CurrentPlayerData.CurrentHealth = BattleManager.Instance.CurrentPlayerData.MaxHealth;
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
    }

    private void EventTakeDamage(params object[] args)
    {
        if (BattleManager.Instance.CurrentPlayerData.CurrentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
