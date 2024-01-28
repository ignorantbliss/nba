using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectCheckpoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void PlayerTouch(Player P)
    {
        Debug.Log("Checkpoint Touched!");
        Animator A = GetComponent<Animator>();
        if (A == null)
        {
            if (transform.parent != null)
            {
                A = transform.parent.gameObject.GetComponent<Animator>();
            }
        }
        if (A != null) A.SetBool("Triggered", true);

    }
}
