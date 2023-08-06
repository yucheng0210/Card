using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStartMenu : UIBase
{
    [SerializeField]
    private Button startButton;

    protected override void Start()
    {
        base.Start();
        startButton.onClick.AddListener(GameStart);
    }
    private void GameStart()
    {
        startButton.onClick.RemoveAllListeners();
        StartCoroutine(SceneController.Instance.Transition("Level1"));
    }
}
