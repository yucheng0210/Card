using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private TextAsset textAsset;
    private EnemyData_SO enemy;
    public enum AttackType
    {
        Attack,
        Shield,
        Effect
    }

    private void Start()
    {
        GetExcelData(textAsset);
        enemy = BattleManager.Instance.EnemyList[0];
        ((UIBattle)UIManager.Instance.FindUI("UIBattle")).ShowEnemyHealth(
            enemy.MaxHealth,
            enemy.CurrentHealth
        );
    }

    private void GetExcelData(TextAsset file)
    {
        BattleManager.Instance.EnemyList.Clear();
        string[] lineData = file.text.Split(new char[] { '\n' });
        for (int i = 1; i < lineData.Length - 1; i++)
        {
            string[] row = lineData[i].Split(new char[] { ',' });
            if (row[1] == "")
                break;
            EnemyData_SO enemyData_SO = ScriptableObject.CreateInstance<EnemyData_SO>();
            enemyData_SO.CharacterName = row[0];
            enemyData_SO.MaxHealth = int.Parse(row[1]);
            enemyData_SO.CurrentHealth = enemyData_SO.MaxHealth;
            BattleManager.Instance.EnemyList.Add(enemyData_SO);
        }
    }
}
