using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ES_1Code : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public GameObject ESBloom;
    void Update()
    {
        this.transform.position+=new Vector3(0,-0.005f,0);  
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "smallBullet(Clone)") {
            ES_1Die();
            ScoreCode.Score = ScoreCode.Score + 100;
        }
        else if (collision.name == "wall4") {
            Destroy(this.gameObject);
        }
        else if (collision.name=="ship2") {
            GameObject.Find("HealthController").SendMessage("HP");
             ES_1Die();
            ScoreCode.Score = ScoreCode.Score + 100;
        }
    }
    public void ES_1Die() {
        Destroy(this.gameObject);
        Instantiate(ESBloom,this.transform.position,Quaternion.identity);
    }
}
