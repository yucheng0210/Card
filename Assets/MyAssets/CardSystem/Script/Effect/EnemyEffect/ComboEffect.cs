using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboEffect : IEffect
{
    private CharacterData characterData;
    private Enemy enemy;
    private int comboCount;
    private bool isCombo;
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        characterData = BattleManager.Instance.IdentifyCharacter(fromLocation);
        enemy = ((EnemyData)characterData).EnemyTrans.GetComponent<Enemy>();
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
        EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
    }
    private void EventTakeDamage(params object[] args)
    {
        if (args.Length < 5)
        {
            return;
        }
        if (args[4] == characterData)
        {
            enemy.AdditionAttackCount = comboCount++;
            enemy.EnemyOnceBattlePositiveList.Add(GetType().Name, comboCount);
            isCombo = true;
        }
        else if (args[5] == characterData && ((CharacterData)args[5]).CurrentShield <= 0)
        {
            comboCount = 0;
            enemy.AdditionAttackCount = comboCount;
        }
    }
    private void EventPlayerTurn(params object[] args)
    {
        if (!isCombo)
        {
            comboCount = 0;
            enemy.AdditionAttackCount = comboCount;
        }
        isCombo = false;
    }
    public string SetTitleText()
    {
        return "連續拳";
    }
    public string SetDescriptionText()
    {
        return "每次攻擊擊中，增加額攻擊次數，破盾後重置。";
    }

}
