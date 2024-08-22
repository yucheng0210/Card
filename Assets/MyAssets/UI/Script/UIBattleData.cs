using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIBattleData : UIBase
{
    [SerializeField]
    private Button exitButton;
    [SerializeField]
    private Button showButton;
    private void Awake()
    {
        Initialize();
    }
    private void Initialize()
    {
        exitButton.onClick.AddListener(Hide);
        showButton.onClick.AddListener(Show);
    }
}
