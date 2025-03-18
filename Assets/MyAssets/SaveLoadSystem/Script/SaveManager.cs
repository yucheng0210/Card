using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.F6))
            SavePlayerData();
        if (Input.GetKeyDown(KeyCode.F7))
            LoadPlayerData();*/
    }

    /* public void SavePlayerData(int saveID)
     {
         SaveTransform();
         Save(GameManager.Instance.PlayerState.CharacterData, saveID.ToString());
     }

     public void LoadPlayerData(int loadID)
     {
         Load(GameManager.Instance.PlayerState.CharacterData, loadID.ToString());
         LoadTransform();
     }

     public void Save(Object data, string key)
     {
         var jsonData = JsonUtility.ToJson(data, true);
         PlayerPrefs.SetString(key, jsonData);
         PlayerPrefs.Save();
     }

     public void Load(Object data, string key)
     {
         if (PlayerPrefs.HasKey(key))
             JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key), data);
     }
 */
    public string HasData(int ID)
    {
        return PlayerPrefs.HasKey(ID.ToString()) ? "Data" + (ID + 1).ToString() : "No Data";
    }

    private void SaveTransform()
    {/*
        GameManager.Instance.PlayerState.Pos =
            GameManager.Instance.PlayerState.gameObject.transform.position;
        GameManager.Instance.PlayerState.Rotation =
            GameManager.Instance.PlayerState.gameObject.transform.rotation;*/
    }

    private void LoadTransform()
    {
        /* GameManager.Instance.PlayerState.gameObject.transform.position =
             GameManager.Instance.PlayerState.Pos;
         GameManager.Instance.PlayerState.gameObject.transform.rotation =
             GameManager.Instance.PlayerState.Rotation;*/
    }
}
