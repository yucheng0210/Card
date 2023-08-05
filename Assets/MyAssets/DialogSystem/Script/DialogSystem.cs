using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject dialog;

    [SerializeField]
    private Text textLabel;

    /*[SerializeField]
    private Image faceImage;*/

    [SerializeField]
    private Text characterName;

    [SerializeField]
    private Sprite playerFace,
        npcFace;

    private int index;

    [SerializeField]
    private float maxTextWaitTime;

    [SerializeField]
    private float currentTextWaitTime;
    private bool textFinished;
    private bool openMenu;
    private bool isTalking;

    [SerializeField]
    private GameObject choiceButton;

    [SerializeField]
    private GameObject choiceManager;

    [SerializeField]
    private bool isQuestDialog = false;

    [SerializeField]
    private int questID;

    [SerializeField]
    private string dialogName;
    private string currentBranchID = "DEFAULT";
    private bool continueBool;
    private bool inSelection;

    public bool OpenMenu
    {
        get { return openMenu; }
        set { openMenu = value; }
    }
    public bool BlockContinue { get; set; }

    private void Start()
    {
        /*EventManager.Instance.AddEventRegister(
            EventDefinition.eventQuestActivate,
            EventQuestActivate
        );
        EventManager.Instance.AddEventRegister(
            EventDefinition.eventQuestCompleted,
            EventQuestCompleted
        );*/
        dialogName = DataManager.Instance.LevelList[DataManager.Instance.LevelID].DialogName;
        EventManager.Instance.AddEventRegister(EventDefinition.eventDialog, EventDialog);
    }

    private void Update()
    {
        if (Time.timeScale == 0 || !isTalking)
            return;
        SetType();
        ContinueDialog();
    }

    private void Initialize()
    {
        if (isQuestDialog)
        {
            //QuestManager.Instance.UpdateActiveQuests();
            //QuestManager.Instance.CheckQuestProgress(questID);
        }
    }

    private void SetType()
    {
        if (index >= DataManager.Instance.DialogList[dialogName].Count)
        {
            if (inSelection)
                return;
            if (continueBool)
                CloseDialog();
            return;
        }
        if (DataManager.Instance.DialogList[dialogName][index].Branch != currentBranchID)
        {
            if (!inSelection)
                index++;
            return;
        }
        switch (DataManager.Instance.DialogList[dialogName][index].Type)
        {
            case "TALK":
                if (continueBool && !inSelection)
                {
                    continueBool = false;
                    if (textFinished)
                    {
                        currentTextWaitTime = maxTextWaitTime;
                        StartCoroutine(SetText());
                    }
                    else
                        currentTextWaitTime = 0;
                }
                break;
            case "CHOICE":
                ChoiceMenu();
                break;
            case "MENU":
                if (continueBool)
                {
                    openMenu = true;
                    index++;
                }
                break;
            case "QUEST":
                //QuestManager.Instance.ActivateQuest(questID);
                index++;
                break;
            case "CALL":
                currentBranchID = DataManager.Instance.DialogList[dialogName][index].Order;
                //if (currentBranchID == "REWARDED")
                //QuestManager.Instance.GetRewards(questID);
                if (continueBool)
                    CloseDialog();
                break;
        }
    }

    private void CloseDialog()
    {
        DestroyChoice();
        dialog.SetActive(false);
        isTalking = false;
        //BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Player);
    }

    private IEnumerator SetText()
    {
        textFinished = false;
        textLabel.text = "";
        SetCharacterInfo();
        for (int i = 0; i < DataManager.Instance.DialogList[dialogName][index].Content.Length; i++)
        {
            textLabel.text += DataManager.Instance.DialogList[dialogName][index].Content[i];
            yield return new WaitForSeconds(currentTextWaitTime);
        }
        textFinished = true;
        index++;
    }

    private void SetCharacterInfo()
    {
        /*switch (dialogList.DialogList[index].TheName)
        {
            case "PLAYER":
                faceImage.sprite = playerFace;
                break;
            case "NPC":
                faceImage.sprite = npcFace;
                break;
        }*/
        characterName.text = DataManager.Instance.DialogList[dialogName][index].TheName;
    }

    private void ChoiceMenu()
    {
        inSelection = true;
        string buttonBranchID = DataManager.Instance.DialogList[dialogName][index].Order;
        string menuType = DataManager.Instance.DialogList[dialogName][index].TheName;
        GameObject choice;
        choice = Instantiate(choiceButton, choiceManager.transform.position, Quaternion.identity);
        choice.transform.SetParent(choiceManager.transform, false);
        choice.GetComponentInChildren<Text>().text = DataManager.Instance.DialogList[dialogName][
            index
        ].Content;
        choice
            .GetComponent<Button>()
            .onClick.AddListener(() => GetBranchID(buttonBranchID, menuType));
        index++;
    }

    private void GetBranchID(string buttonBranchID, string menuType)
    {
        //if (buttonBranchID == "ACTIVATE")
        //QuestManager.Instance.ActivateQuest(questID);
        if (buttonBranchID == "BATTLE")
        {
            currentBranchID += buttonBranchID;
            BattleManager.Instance.ChangeTurn(BattleManager.BattleType.BattleInitial);
        }
        else
            currentBranchID = buttonBranchID;
        DestroyChoice();
        inSelection = false;
        continueBool = true;
        switch (menuType)
        {
            case "CLOSE":
                CloseDialog();
                break;
            case "END":
                UIManager.Instance.ShowUI("UIMap");
                BattleManager.Instance.ChangeTurn(BattleManager.BattleType.None);
                break;
        }
        if (BattleManager.Instance.MyBattleType == BattleManager.BattleType.Dialog)
        {
            BattleManager.Instance.CurrentLocationID = int.Parse(buttonBranchID);
            BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Explore);
        }
    }

    private void DestroyChoice()
    {
        for (int i = 0; i < choiceManager.transform.childCount; i++)
        {
            Destroy(choiceManager.transform.GetChild(i).gameObject);
        }
    }

    private void ContinueDialog()
    {
        if (
            (
                Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetMouseButtonDown(0)
            // || Input.GetButtonDown("A")
            ) && !BlockContinue
        )
            continueBool = true;
    }

    /* private void EventQuestActivate(params object[] args)
     {
         currentBranchID = "ACTIVE";
     }

     private void EventQuestCompleted(params object[] args)
     {
         currentBranchID = "COMPLETED";
     }*/

    private void EventDialog(params object[] args)
    {
        textFinished = false;
        textLabel.text = "";
        currentTextWaitTime = maxTextWaitTime;
        continueBool = true;
        index = 0;
        textFinished = true;
        SetCharacterInfo();
        Initialize();
        dialog.SetActive(true);
        isTalking = true;
    }
}
