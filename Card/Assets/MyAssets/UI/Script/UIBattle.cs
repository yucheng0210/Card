using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBattle : UIBase
{
    [SerializeField]
    private Text actionPointText;

    [SerializeField]
    private Text shieldText;

    [SerializeField]
    private Slider healthSlider;

    [SerializeField]
    private Text healthText;

    [SerializeField]
    private Slider enemyHealthSlider;

    [SerializeField]
    private Text enemyHealthText;

    [SerializeField]
    private RectTransform enemyHealthRect;

    [SerializeField]
    private RectTransform enemyHurtRect;

    public Text ActionPointText
    {
        get { return actionPointText; }
        set { actionPointText = value; }
    }
    public Text ShieldText
    {
        get { return shieldText; }
        set { shieldText = value; }
    }
    public Slider HealthSlider
    {
        get { return healthSlider; }
        set { healthSlider = value; }
    }
    public Text HealthText
    {
        get { return healthText; }
        set { healthText = value; }
    }

    private void Update()
    {
        enemyHurtRect.anchorMax = new Vector2(
            Mathf.Lerp(enemyHurtRect.anchorMax.x, enemyHealthRect.anchorMax.x, Time.deltaTime * 5),
            enemyHurtRect.anchorMax.y
        );
    }

    public void ShowEnemyHealth(int maxHealth, int currentHealth)
    {
        enemyHealthSlider.value = (currentHealth / (float)maxHealth);
        enemyHealthText.text = currentHealth.ToString() + "/" + maxHealth.ToString();
    }
}
