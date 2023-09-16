using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemOnWorld : MonoBehaviour
{
    [SerializeField]
    private int itemIndex;
    private GameObject clue;
    private bool isPickUp;

    private void Awake()
    {
        clue = transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && clue.activeSelf)
            isPickUp = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            clue.SetActive(true);
            if (isPickUp)
            {
                if (gameObject.CompareTag("Item"))
                    BackpackManager.Instance.AddItem(itemIndex, DataManager.Instance.Backpack);
                else if (gameObject.CompareTag("Money"))
                    BackpackManager.Instance.AddMoney(100);
                Destroy(gameObject);
                clue.SetActive(false);
                isPickUp = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            clue.SetActive(false);
    }
}
