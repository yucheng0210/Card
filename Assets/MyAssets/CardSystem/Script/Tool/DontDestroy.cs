using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroy : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
        EventManager.Instance.AddEventRegister(EventDefinition.eventReloadGame, EventReloadGame);
    }
    private void EventReloadGame(params object[] args)
    {
        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
    }

}
