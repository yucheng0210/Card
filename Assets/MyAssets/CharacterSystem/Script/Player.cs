using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField]
    private int currentPlayerID;
    [SerializeField]
    private Transform checkerboardTrans;

    private void Start()
    {
        DataManager.Instance.PlayerList[currentPlayerID].CurrentHealth = DataManager
            .Instance
            .PlayerList[currentPlayerID].MaxHealth;
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
        EventManager.Instance.AddEventRegister(EventDefinition.eventBattleInitial, EventBattleInitial);
        BattleManager.Instance.PlayerTrans = transform;
        BattleManager.Instance.CheckerboardTrans = checkerboardTrans;
    }

    private void EventTakeDamage(params object[] args)
    {
        if (DataManager.Instance.PlayerList[currentPlayerID].CurrentHealth <= 0)
            Destroy(gameObject);
    }
    private void EventBattleInitial(params object[] args)
    {
        string[] pos = DataManager.Instance.LevelList[DataManager.Instance.LevelID].PlayerStartPos.Split(' ');
        int x = int.Parse(pos[0]);
        int y = int.Parse(pos[1]);
        int currentPos = x + y * 8;
        transform.SetParent(checkerboardTrans.GetChild(currentPos));
        GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
    }
}
