using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Unity.VisualScripting;
public class UIIllustratedBook : UIBase
{
    [SerializeField]
    private List<GameObject> allBackgroundList = new List<GameObject>();
    [SerializeField]
    private List<Button> allLabelList = new List<Button>();
    [SerializeField]
    private Button showButton;
    [SerializeField]
    private Button exitButton;
    [Header("藥水")]
    [SerializeField]
    private List<Button> potionButtonList = new List<Button>();
    [SerializeField]
    private List<Sprite> potionSpriteList = new List<Sprite>();
    [SerializeField]
    private Image potionExhibitImage;
    [Header("服裝卡片")]
    [SerializeField]
    private List<Button> suitCardButtonList = new List<Button>();
    [SerializeField]
    private List<Sprite> suitCardSpriteList = new List<Sprite>();
    [SerializeField]
    private Image suitCardExhibitImage;
    [Header("戰鬥卡片")]
    [SerializeField]
    private List<Button> battleCardButtonList = new List<Button>();
    [SerializeField]
    private List<Sprite> battleCardSpriteList = new List<Sprite>();
    [SerializeField]
    private Image battleCardExhibitImage;
    [SerializeField]
    private Button nextPageButton;
    private int pageCount = 1;
    private void Awake()
    {
        Initialize();
    }
    private void Initialize()
    {
        showButton.onClick.AddListener(Show);
        exitButton.onClick.AddListener(Hide);
        for (int i = 0; i < allLabelList.Count; i++)
        {
            int id = i;
            allLabelList[i].onClick.AddListener(() => Refresh(allBackgroundList[id]));
        }
        SetButtonListener(potionButtonList, potionSpriteList, potionExhibitImage);
        SetButtonListener(suitCardButtonList, suitCardSpriteList, suitCardExhibitImage);
        SetButtonListener(battleCardButtonList, battleCardSpriteList, battleCardExhibitImage);
        nextPageButton.onClick.AddListener(NextPage);
    }
    private void NextPage()
    {
        for (int i = 0; i < battleCardButtonList.Count; i++)
        {
            battleCardButtonList[i].gameObject.SetActive(false);
        }
        for (int i = (pageCount - 1) * 9; i < pageCount * 9; i++)
        {
            battleCardButtonList[i].gameObject.SetActive(true);
        }
        if (pageCount == 3)
        {
            pageCount = 1;
        }
        else
        {
            pageCount++;
        }
    }
    private void SetButtonListener(List<Button> buttonList, List<Sprite> spriteList, Image exhibitImage)
    {
        for (int i = 0; i < buttonList.Count; i++)
        {
            int index = i;
            buttonList[index].onClick.AddListener(() => exhibitImage.sprite = spriteList[index]);
        }
    }
    private void Refresh(GameObject background)
    {
        for (int i = 0; i < allBackgroundList.Count; i++)
        {
            allBackgroundList[i].SetActive(false);
        }
        background.SetActive(true);
    }

}
