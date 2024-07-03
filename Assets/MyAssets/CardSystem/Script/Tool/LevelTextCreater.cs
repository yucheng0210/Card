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
        List<Vector2> removeList = new List<Vector2>();
        for (int i = 0; i < 333; i++)
        {
            int enemy = Random.Range(4, 6);
            int terrain = Random.Range(3, 7);
            s += "1001,小怪,";
            for (int j = 0; j < enemy; j++)
            {
                int x = Random.Range(1, maxX + 1);
                int y = Random.Range(1, maxY + 1);
                if (removeList.Contains(new Vector2(x, y)))
                {
                    j--;
                    continue;
                }
                int enemyID = Random.Range(1001, 1005);
                if (j == enemy - 1)
                    s += x + " " + y + "=" + enemyID + ",";
                else
                    s += x + " " + y + "=" + enemyID + ";";
                removeList.Add(new Vector2(x, y));
            }
            s += ",BATTLE,0 0,";
            for (int k = 0; k < terrain; k++)
            {
                int x = Random.Range(1, maxX + 1);
                int y = Random.Range(1, maxY + 1);
                if (removeList.Contains(new Vector2(x, y)))
                {
                    k--;
                    continue;
                }
                int terrainID = Random.Range(1001, 1004);
                if (k == terrain - 1)
                    s += x + " " + y + "=" + terrainID + "\r\n";
                else
                    s += x + " " + y + "=" + terrainID + ";";
                removeList.Add(new Vector2(x, y));
            }
            removeList.Clear();
        }
        Debug.Log(s);
    }
}
