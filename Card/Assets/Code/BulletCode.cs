using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCode : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.parent = null;
        this.transform.position += new Vector3(0,0.01f,0);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "wall3") {
            Destroy(this.gameObject);
        }
        else if (collision.name=="ES_1(Clone)") {
            Destroy(this.gameObject);
        }
    }
}
