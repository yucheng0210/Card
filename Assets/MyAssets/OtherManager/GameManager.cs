using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public void StartGame()
    {
        EventManager.Instance.DispatchEvent(EventDefinition.eventStartGame);
    }
}
