using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetLevel : MonoBehaviour
{
    public void DeleteLevel()
    {
        GameObject[] Objs = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject G in Objs)
        {
            if (G.layer == 10)
            {
                if (G.name != "StartSpot")
                {
                    GameObject.Destroy(G);
                }
            }
            if (G.layer == 12)
            {                
                GameObject.Destroy(G);                
            }
        }
    }

    public void ClearSoft()
    {
        GameObject[] Objs = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject G in Objs)
        {
            if (G.layer == 10)
            {
                if (G.name != "StartSpot")
                {
                    if (!G.name.Contains("Stone"))
                        GameObject.Destroy(G);
                }
            }
            if (G.layer == 12)
            {
                GameObject.Destroy(G);
            }
        }
    }
}
