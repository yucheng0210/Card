using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIGameOver : UIBase
{
    [SerializeField]
    private Button startMenuButton;
    [SerializeField]
    private Image titleImage;
    [SerializeField]
    private List<Sprite> titleSpriteList = new List<Sprite>();
    protected override void Start()
    {
        base.Start();
        EventManager.Instance.AddEventRegister(EventDefinition.eventGameOver, EventGameOver);
        startMenuButton.onClick.AddListener(() => StartCoroutine(SceneController.Instance.Transition("StartMenu")));
    }
    private void EventGameOver(params object[] args)
    {
        Show();
        if ((bool)args[0])
        {
            titleImage.sprite = titleSpriteList[0];
        }
        else
        {
            titleImage.sprite = titleSpriteList[1];
        }
        AudioManager.Instance.BGMSource.Pause();
        AudioManager.Instance.SEAudio(8);
    }
}
