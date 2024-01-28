using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    public GameObject HeartContainer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        WakeUp();
    }

    public void WakeUp()
    {
        //Prime all hearts...
        foreach (Transform T in HeartContainer.transform)
        {
            Animator A = T.gameObject.GetComponent<Animator>();
            if (A != null)
            {
                A.SetTrigger("Reactivate");
            }
        }
    }
}
