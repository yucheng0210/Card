using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    private Transform canvasTrans;
    public List<UIBase> UIList { get; set; }

    protected override void Awake()
    {
        base.Awake();
        UIList = new List<UIBase>();
    }

    public UIBase FindUI(string uIName)
    {
        for (int i = 0; i < UIList.Count; i++)
        {
            if (UIList[i].GetType().Name == uIName)
                return UIList[i];
        }
        return null;
    }

    public void CloseAllUI()
    {
        for (int i = 0; i < UIList.Count; i++)
        {
            UIList[i].gameObject.SetActive(false);
        }
    }

    public IEnumerator FadeOutIn(CanvasGroup canvasGroup, float fadeTime, float waitTime)
    {
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.unscaledDeltaTime / fadeTime;
            yield return null;
        }
        yield return new WaitForSecondsRealtime(waitTime);
        while (canvasGroup.alpha != 0)
        {
            canvasGroup.alpha -= Time.unscaledDeltaTime / fadeTime;
            yield return null;
        }
        Destroy(canvasGroup.gameObject);
    }

    public void ShowActionPointUI(int currentActionPoint, int maxActionPoint)
    {
        Text actionPointText = ((UIBattle)FindUI("UIBattle")).ActionPointText;
        actionPointText.text = currentActionPoint.ToString() + "/" + maxActionPoint.ToString();
    }

    public void ShowHealthUI(int maxHealth, int currentHealth)
    {
        Slider healthSlider = ((UIBattle)FindUI("UIBattle")).HealthSlider;
        Text healthText = ((UIBattle)FindUI("UIBattle")).HealthText;
        healthSlider.value = currentHealth / maxHealth;
        healthText.text = currentHealth.ToString() + "/" + maxHealth.ToString();
    }

    public void ShowShieldUI(int shieldPoint)
    {
        Text shieldText = ((UIBattle)FindUI("UIBattle")).ShieldText;
        shieldText.text = shieldPoint.ToString();
    }
}
