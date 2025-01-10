using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterData
{
    private int maxHealth;
    private int currentHealth;
    private int currentShield;
    public int CharacterID { get; set; }
    public string CharacterName { get; set; }
    public string CharacterPos { get; set; }
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
            if (currentHealth > MaxHealth)
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
    public int StartSkill { get; set; }
    public int DamageReduction { get; set; }
    public int DamageLimit { get; set; }
}
