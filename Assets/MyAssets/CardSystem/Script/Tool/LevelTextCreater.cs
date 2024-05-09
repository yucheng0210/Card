using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTextCreater : MonoBehaviour
{
    private int maxX = 7;
    private int maxY = 4;
    private void Start()
    {
        string s = "";
        for (int i = 0; i < 40; i++)
        {
            int enemy = Random.Range(1, 4);
            int terrain = Random.Range(1, 6);
            List<int> removeXList = new List<int>();
            List<int> removeYList = new List<int>();
            s += "1001,小怪,";
            for (int j = 0; j < enemy; j++)
            {
                int x = Random.Range(1, maxX + 1);
                int y = Random.Range(1, maxY + 1);
                if (removeXList.Contains(x) && removeYList.Contains(y))
                {
                    j--;
                    continue;
                }
                int enemyID = Random.Range(1001, 1006);
                if (j == enemy - 1)
                    s += x + " " + y + "=" + enemyID + ",";
                else
                    s += x + " " + y + "=" + enemyID + ";";
                removeXList.Add(x);
                removeYList.Add(y);
            }
            s += ",BATTLE,0 0,";
            for (int k = 0; k < terrain; k++)
            {
                int x = Random.Range(1, maxX + 1);
                int y = Random.Range(1, maxY + 1);
                if (removeXList.Contains(x) && removeYList.Contains(y))
                {
                    k--;
                    continue;
                }
                int terrainID = Random.Range(1001, 1004);
                if (k == terrain - 1)
                    s += x + " " + y + "=" + terrainID + "\r\n";
                else
                    s += x + " " + y + "=" + terrainID + ";";
                removeXList.Add(x);
                removeYList.Add(y);
            }
        }
        Debug.Log(s);
    }
}
