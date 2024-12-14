using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhantomEffect : IEffect
{
    private EnemyData enemyData;
    private int attackCount = 2;
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        enemyData = (EnemyData)BattleManager.Instance.IdentifyCharacter(fromLocation);
        BattleManager.Instance.AddMinions(2009, value, fromLocation);
        EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
    }
    private void EventPlayerTurn(params object[] args)
    {
        attackCount = 2;
    }
    private void EventTakeDamage(params object[] args)
    {
        if (args[5] == enemyData)
        {
            attackCount--;
            if (attackCount <= 0)
            {
                BattleManager.Instance.ClearAllMinions();
            }
        }
    }
    public string SetTitleText()
    {
        return "幻象";
    }
    public string SetDescriptionText()
    {
        return "召喚幻象。";
    }

}
