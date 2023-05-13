using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESExplode : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject,0.65f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
