using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkButton : MonoBehaviour
{
    public GameObject button;
    public GameObject talkUI;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
            button.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            button.SetActive(false);
            talkUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (button.activeSelf && Input.GetKeyDown(KeyCode.E))
            talkUI.SetActive(true);
    }
}
