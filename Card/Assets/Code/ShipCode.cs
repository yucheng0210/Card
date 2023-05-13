using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ShipCode : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

  
    public GameObject smallBullet;
    public GameObject ShipBloom;
    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position += new Vector3(0.01f, 0, 0);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position += new Vector3(-0.01f, 0, 0);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(smallBullet, this.transform.position, Quaternion.identity);
        }
    }
    public void ShipDie()
    {
        Destroy(this.gameObject);
        Instantiate(ShipBloom,this.transform.position, Quaternion.identity);
    }
}
