using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private int currentHealth;

    private void Start()
    {
        currentHealth = DataManager.Instance.PlayerList[1].MaxHealth;
        UIManager.Instance.ShowHealthUI(
            DataManager.Instance.PlayerList[1].MaxHealth,
            currentHealth
        );
    }
}
