using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCode : MonoBehaviour
{
    public static int Score;
    public Text ShowScore;
    void Start()
    {
        Score = 0;
    }

   
    void Update()
    {
        ShowScore.text = Score.ToString();
    }
}
