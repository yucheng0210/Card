using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPCode : MonoBehaviour
{
    public const int maxHealth = 250;
    public int currentHealth = maxHealth;
    public RectTransform LifePoint, Hurt;
    void Update()
    {
        LifePoint.sizeDelta = new Vector2(currentHealth, LifePoint.sizeDelta.y);
        if (LifePoint.sizeDelta.x < Hurt.sizeDelta.x)
        {
            Hurt.sizeDelta += new Vector2(-2, 0) * Time.deltaTime * 10;
        }
        if (Hurt.sizeDelta.x<= 0)
        {
            GameObject.Find("ship2").SendMessage("ShipDie");
        }
    }
    public void HP(){
        currentHealth = currentHealth - 20;      
    }
}
