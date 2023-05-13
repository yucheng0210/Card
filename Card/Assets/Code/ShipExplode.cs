using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipExplode : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() {     
        Destroy(this.gameObject, 0.85f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
