using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ES_1CC : MonoBehaviour
{
    public GameObject ES_1;
    void Start()
    {
        InvokeRepeating("ES_1Creater", 1, 1);
    }

    // Update is called once per frame
    public void ES_1Creater() {
        int ES_1Num;
        ES_1Num = Random.Range(0, 3);
        for (int i = 0; i < ES_1Num; i++) {
            float x;
            x = Random.Range(-6, 6);
            Instantiate(ES_1, new Vector3(x, 2.8f, 0), Quaternion.identity);
        }
    }
}

