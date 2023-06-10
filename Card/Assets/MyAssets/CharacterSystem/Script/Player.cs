using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField]
    private int currentPlayerID;

    private void Start()
    {
        DataManager.Instance.PlayerList[currentPlayerID].CurrentHealth = DataManager
            .Instance
            .PlayerList[currentPlayerID].MaxHealth;
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
    }

    private void EventTakeDamage(params object[] args)
    {
        if (DataManager.Instance.PlayerList[currentPlayerID].CurrentHealth <= 0)
            Destroy(gameObject);
    }
}
