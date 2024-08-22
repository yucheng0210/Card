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
