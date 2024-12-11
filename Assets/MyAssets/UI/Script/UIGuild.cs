using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGuild : UIBase
{
    [SerializeField]
    private Button mainQuestButton;
    [SerializeField]
    private Button applyButton;
    [SerializeField]
    private List<int> rewardList = new List<int>();
    [SerializeField]
    private Button exitButton;
    private bool isReceiveReward;
    protected override void Start()
    {
        base.Start();
        Initialize();
    }
    private void Initialize()
    {
        mainQuestButton.onClick.AddListener(() => ShowQuest(mainQuestButton));
        exitButton.onClick.AddListener(() => UIManager.Instance.ShowUI(GetType().Name));
    }
    private void ShowQuest(Button quest)
    {
        for (int i = 0; i < quest.transform.childCount; i++)
        {
            GameObject go = quest.transform.GetChild(i).gameObject;
            Button goButton = quest.transform.GetChild(i).GetComponent<Button>();
            int chapterID = i + 1;
            goButton.onClick.AddListener(() => RefreshQuestInfo(goButton, chapterID));
            go.SetActive(!go.activeSelf);
        }
        applyButton.gameObject.SetActive(!applyButton.gameObject.activeSelf);
    }
    private void RefreshQuestInfo(Button quest, int chapterID)
    {
        // 隱藏 mainQuestButton 的所有子物件
        int mainChildCount = mainQuestButton.transform.childCount;
        for (int i = 0; i < mainChildCount; i++)
        {
            Transform trans = mainQuestButton.transform.GetChild(i);
            int subChildCount = trans.childCount;
            for (int j = 0; j < subChildCount; j++)
            {
                trans.GetChild(j).gameObject.SetActive(false);
            }
        }

        // 顯示 quest 的所有子物件
        int questChildCount = quest.transform.childCount;
        for (int i = 0; i < questChildCount; i++)
        {
            GameObject go = quest.transform.GetChild(i).gameObject;
            go.SetActive(true);
        }

        // 移除 applyButton 的所有現有 Listeners
        applyButton.onClick.RemoveAllListeners();

        // 設定 applyButton 的行為和文字
        Text applyText = applyButton.GetComponentInChildren<Text>();
        if (chapterID == MapManager.Instance.ChapterCount)
        {
            if (isReceiveReward)
            {
                applyText.text = "已領取";
            }
            else
            {
                int rewardID = chapterID - 1;
                applyText.text = "領取";
                applyButton.onClick.AddListener(() =>
                {
                    GetPotion(DataManager.Instance.PotionList[rewardList[rewardID]], applyText);
                    isReceiveReward = true;
                    applyButton.onClick.RemoveAllListeners(); // 避免重複添加 Listener
                });
            }
        }
        else if (chapterID == MapManager.Instance.ChapterCount + 1)
        {
            applyText.text = "接取";
            applyButton.onClick.AddListener(() =>
            {
                UI.SetActive(false);
                UIManager.Instance.HideUI("UISkyIsland");
                EventManager.Instance.DispatchEvent(EventDefinition.eventNextChapter);
                applyButton.onClick.RemoveAllListeners();
            });
        }
        else if (chapterID < MapManager.Instance.ChapterCount)
        {
            applyText.text = "已領取";
        }
        else
        {
            applyText.text = "???";
        }
    }
    private void GetPotion(Potion potionItem, Text applyText)
    {
        applyText.text = "已領取";
        DataManager.Instance.PotionBag.Add(potionItem);
        applyButton.onClick.RemoveAllListeners();
    }
}
