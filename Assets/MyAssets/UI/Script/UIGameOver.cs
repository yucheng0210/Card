using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIGameOver : UIBase
{
    [SerializeField]
    private Button startMenuButton;
    private void Awake()
    {
        EventManager.Instance.AddEventRegister(EventDefinition.eventGameOver, EventGameOver);
        startMenuButton.onClick.AddListener(() => StartCoroutine(SceneController.Instance.Transition("StartMenu")));
    }
    private void EventGameOver(params object[] args)
    {
        Show();
    }
}
