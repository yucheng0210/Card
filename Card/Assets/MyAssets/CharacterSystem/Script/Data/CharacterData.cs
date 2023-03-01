using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData
{
    private int maxHealth;
    private int currentHealth;
    private int currentShield;
    public string CharacterName { get; set; }
    public int MaxHealth
    {
        get { return maxHealth; }
        set
        {
            maxHealth = value;
            if (maxHealth < 0)
                maxHealth = 0;
            if (maxHealth > 999)
                maxHealth = 999;
        }
    }
    public int CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            currentHealth = value;
            if (currentHealth < 0)
                currentHealth = 0;
            if (currentHealth > maxHealth)
                currentHealth = maxHealth;
        }
    }
    public int CurrentShield
    {
        get { return currentShield; }
        set
        {
            currentShield = value;
            if (currentShield < 0)
                currentShield = 0;
            if (currentShield > 999)
                currentShield = 999;
        }
    }
}
