using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PotionItem : MonoBehaviour
{
    [SerializeField]
    private Text priceText;
    [SerializeField]
    private Text infoTitleText;
    [SerializeField]
    private Text infoDescriptionText;
    [SerializeField]
    private GameObject infoGameObject;
    [SerializeField]
    private Transform statusClueTrans;
    public Image PotionImage { get; set; }
    public Button PotionButton { get; set; }
    public EventTrigger PotionEventTrigger { get; set; }
    public Canvas PotionCanvas { get; set; }
    public Text PriceText { get { return priceText; } set { priceText = value; } }
    public Text InfoTitleText { get { return infoTitleText; } set { infoTitleText = value; } }
    public Text InfoDescriptionText { get { return infoDescriptionText; } set { infoDescriptionText = value; } }
    public GameObject InfoGameObject { get { return infoGameObject; } set { infoGameObject = value; } }
    public Transform StatusClueTrans { get { return statusClueTrans; } set { statusClueTrans = value; } }
    private void Awake()
    {
        PotionImage = GetComponent<Image>();
        PotionButton = GetComponent<Button>();
        PotionEventTrigger = GetComponent<EventTrigger>();
        PotionCanvas = InfoGameObject.GetComponent<Canvas>();
    }
}
