using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFolderShow : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Show(GameObject G)
    {
        foreach(Transform T in gameObject.transform)
        {            
            foreach(Transform Tx in T)
            {
                if (Tx.gameObject != G)
                {
                    Tx.gameObject.SetActive(false);
                }
                else
                {
                    Tx.gameObject.SetActive(true);
                }
            }
        }
    }
}
