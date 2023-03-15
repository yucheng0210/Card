using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField]
    private TextAsset textAsset;
    private int currentHealth;

    private void Start()
    {
        GetExcelData(textAsset);
        currentHealth = BattleManager.Instance.PlayerList[0].MaxHealth;
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Player);
        UIManager.Instance.ShowHealthUI(
            BattleManager.Instance.PlayerList[0].MaxHealth,
            currentHealth
        );
    }

    private void GetExcelData(TextAsset file)
    {
        BattleManager.Instance.PlayerList = new List<PlayerData_SO>();
        BattleManager.Instance.PlayerList.Clear();
        //index = 0;
        string[] lineData = file.text.Split(new char[] { '\n' });
        for (int i = 1; i < lineData.Length - 1; i++)
        {
            string[] row = lineData[i].Split(new char[] { ',' });
            if (row[1] == "")
                break;
            PlayerData_SO playerData_SO = ScriptableObject.CreateInstance<PlayerData_SO>();
            playerData_SO.CharacterName = row[0];
            playerData_SO.MaxHealth = int.Parse(row[1]);
            playerData_SO.CurrentShield = int.Parse(row[2]);
            playerData_SO.MaxActionPoint = int.Parse(row[3]);
            BattleManager.Instance.PlayerList.Add(playerData_SO);
        }
    }
}
