using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadOnClicked : MonoBehaviour
{
    private Button button;
    private Text dataName;
    private Text dataTime;
    private int loadID;
    #region "SaveManager"
    /*    private void Awake()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<Text>();
    }

    private void Start()
    {
        button.onClick.AddListener(LoadSuccessfully);
        loadID = gameObject.transform.GetSiblingIndex();
        buttonText.text = SaveManager.Instance.HasData(loadID);
    }

    private void OnEnable()
    {
        buttonText.text = SaveManager.Instance.HasData(loadID);
    }

    private void LoadSuccessfully()
    {
        SaveManager.Instance.LoadPlayerData(loadID);
        buttonText.text = SaveManager.Instance.HasData(loadID);
    }*/
    #endregion
    private void Awake()
    {
        button = GetComponent<Button>();
        dataName = transform.GetChild(0).GetComponent<Text>();
        dataTime = transform.GetChild(1).GetComponent<Text>();
    }

    private void Start()
    {
        button.onClick.AddListener(LoadData);
    }

    private void OnEnable()
    {
        loadID = transform.GetSiblingIndex();
        dataName.text = SaveLoadManager.Instance.GetDataName(loadID);
        dataTime.text = SaveLoadManager.Instance.GetDataTime(loadID);
    }

    private void LoadData()
    {
        SaveLoadManager.Instance.Load(loadID);
        Time.timeScale = 1;
    }
}
