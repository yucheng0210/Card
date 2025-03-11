using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIOption : UIBase
{
    [SerializeField]
    private Button exitButton;
    [SerializeField]
    private Button returnButton;
    [SerializeField]
    private Button showButton;
    private void Awake()
    {
        Initialize();
    }
    private void Initialize()
    {
        exitButton.onClick.AddListener(() => Application.Quit());
        returnButton.onClick.AddListener(Hide);
        showButton.onClick.AddListener(Show);
    }
}
