using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SaveOnClicked : MonoBehaviour
{
    private Button button;
    private Text dataName;
    private Text dataTime;
    private int saveID;
   // private ClueMenu clueMenu;

    #region "SaveManager"
    /*
    private void Awake()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<Text>();
    }

    private void Start()
    {
        button.onClick.AddListener(SaveSuccessfully);
        saveID = gameObject.transform.GetSiblingIndex();
        buttonText.text = SaveManager.Instance.HasData(saveID);
    }

    private void OnEnable()
    {
        buttonText.text = SaveManager.Instance.HasData(saveID);
    }

    private void SaveSuccessfully()
    {
        SaveManager.Instance.SavePlayerData(saveID);
        buttonText.text = SaveManager.Instance.HasData(saveID);
    }
    */
    #endregion
    private void Awake()
    {
        button = GetComponent<Button>();
        dataName = transform.GetChild(0).GetComponent<Text>();
        dataTime = transform.GetChild(1).GetComponent<Text>();
        //clueMenu = transform.root.GetComponent<ClueMenu>();
    }

    private void Start()
    {
        button.onClick.AddListener(SaveData);
    }

    private void OnEnable()
    {
        saveID = transform.GetSiblingIndex();
        dataName.text = SaveLoadManager.Instance.GetDataName(saveID);
        dataTime.text = SaveLoadManager.Instance.GetDataTime(saveID);
    }

    private void SaveData()
    {
        if (SaveLoadManager.Instance.GetDataName(saveID) != "NO DATA")
        {
          /*  clueMenu.Show();
            clueMenu.CurrentSaveID = saveID;*/
        }
        else
        {
           // SaveLoadManager.Instance.Save(saveID);
            if (SceneManager.GetActiveScene().name == "StartMenu")
            {
                SaveLoadManager.Instance.Load(saveID);
            }
            Time.timeScale = 1;
        }
    }
}
