using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ResurrectionEffect : IEffect
{
    private int reciprocalCount = 1;
    private int recoverCount;
    private string id;
    private EnemyData enemyData;
    public void ApplyEffect(int value, string target)
    {
        enemyData = BattleManager.Instance.CurrentEnemyList[target];
        Enemy enemy = enemyData.EnemyTrans.GetComponent<Enemy>();
        GameObject enemyEffect = enemy.EnemyEffectImage;
        Image enemyEffectImage = enemyEffect.GetComponent<Image>();
        if (enemyData.CurrentHealth <= 0)
        {
            enemyData.PassiveSkills.Remove("ResurrectionEffect");
            enemyEffectImage.sprite = EffectFactory.Instance.CreateEffect("ResurrectionEffect").SetIcon();
            enemyEffect.SetActive(true);
            recoverCount = enemyData.MaxHealth * value / 100;
            id = target;
            EventManager.Instance.AddEventRegister(EventDefinition.eventEnemyTurn, EventEnemyTurn);
            EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
        }
    }
    private void EventEnemyTurn(params object[] args)
    {
        reciprocalCount--;
        if (reciprocalCount <= 0)
        {
            Enemy enemy = enemyData.EnemyTrans.GetComponent<Enemy>();
            enemy.IsDeath = false;
            BattleManager.Instance.Recover(enemyData, recoverCount, id);
            enemy.MyAnimator.SetTrigger("isResurrection");
            EventManager.Instance.DispatchEvent(EventDefinition.eventMove);
            EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
            EventManager.Instance.RemoveEventRegister(EventDefinition.eventEnemyTurn, EventEnemyTurn);
        }
    }
    public Sprite SetIcon()
    {
        return Resources.Load<Sprite>("EffectImage/Resurrection");
    }
}
