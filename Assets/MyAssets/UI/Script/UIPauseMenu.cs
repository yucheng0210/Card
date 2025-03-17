using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIPauseMenu : UIBase
{
  [SerializeField]
  private Button continueButton;
  [SerializeField]
  private Button optionButton;
  [SerializeField]
  private Button startMenuButton;
  private void Awake()
  {
    continueButton.onClick.AddListener(Hide);
    startMenuButton.onClick.AddListener(() =>
    {
      StartCoroutine(SceneController.Instance.Transition("StartMenu"));
      EventManager.Instance.DispatchEvent(EventDefinition.eventReloadGame);
    });
  }
  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.Escape) && BattleManager.Instance.MyBattleType != BattleManager.BattleType.DrawCard)
    {
      Show();
    }
  }

}
