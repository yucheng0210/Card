using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SniperEffect : IEffect
{
    private EnemyData enemyData;
    private int attackMultiplier;

    public void ApplyEffect(int value, string target)
    {
        if (BattleManager.Instance.CurrentEnemyList.TryGetValue(target, out enemyData))
        {
            attackMultiplier = value;
            EventManager.Instance.AddEventRegister(EventDefinition.eventMove, EventMove);
            EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
            enemyData.PassiveSkills.Remove(GetType().Name);
        }
        else
            Debug.LogWarning($"Target {target} not found in CurrentEnemyList.");
    }

    private void EventMove(params object[] args)
    {
        RefreshAttack();
    }

    private void EventPlayerTurn(params object[] args)
    {
        RefreshAttack();
    }
    private void RefreshAttack()
    {
        // Assuming args[4] contains the attacking enemy data
        Dictionary<string, EnemyData> currentEnemyList = BattleManager.Instance.CurrentEnemyList;
        if (!currentEnemyList.ContainsValue(enemyData))
        {
            EventManager.Instance.RemoveEventRegister(EventDefinition.eventMove, EventMove);
            EventManager.Instance.RemoveEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
            return;
        }
        string defenderLocation = enemyData.EnemyTrans.GetComponent<Enemy>().EnemyLocation;
        int distance = (int)BattleManager.Instance.GetDistance(defenderLocation);
        enemyData.CurrentAttack = enemyData.MinAttack + Mathf.RoundToInt(enemyData.MinAttack * (distance - 1) * (attackMultiplier / 100f));
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }
    public Sprite SetIcon()
    {
        // Implement SetIcon method or remove if not needed
        return Resources.Load<Sprite>("EffectImage/SniperEffect");
    }

    public string SetTitleText()
    {
        throw new System.NotImplementedException();
    }

    public string SetDescriptionText()
    {
        throw new System.NotImplementedException();
    }
}
