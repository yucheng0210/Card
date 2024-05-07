using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTextCreater : MonoBehaviour
{
    private int maxX = 7;
    private int maxY = 4;
    private void Start()
    {
        for (int i = 0; i < 40; i++)
        {
            string s = "";
            int enemy = Random.Range(1, 4);
            int terrain = Random.Range(1, 6);
            List<int> removeXList = new List<int>();
            List<int> removeYList = new List<int>();
            s += "敵人：";
            for (int j = 0; j < enemy; j++)
            {
                int x = Random.Range(0, maxX + 1);
                int y = Random.Range(0, maxY + 1);
                if (removeXList.Contains(x) && removeYList.Contains(y))
                    continue;
                int enemyID = Random.Range(1001, 1006);
                s += x + " " + y + "=" + enemyID + ";";
            }
            s += "," + "地形：";
            for (int k = 0; k < terrain; k++)
            {
                int x = Random.Range(0, maxX + 1);
                int y = Random.Range(0, maxY + 1);
                if (removeXList.Contains(x) && removeYList.Contains(y))
                    continue;
                int terrainID = Random.Range(1001, 1004);
                s += x + " " + y + "=" + terrainID + ";";
            }
            Debug.Log(s);
        }
    }
}
