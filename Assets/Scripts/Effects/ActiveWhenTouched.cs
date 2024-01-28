using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveWhenTouched : Grounded
{ 
    public override void GroundHit(Player P)
    {
        Animator A = null;
        GameObject Pal = gameObject;
        while (Pal != null)
        {
            //Debug.Log("Checking " + Pal.name + " For Animator...");
            A = Pal.GetComponent<Animator>();
            if (A == null)
            {
                if (Pal.transform.parent != null)
                    Pal = Pal.transform.parent.gameObject;
                else
                    Pal = null;
            }
            else
                break;

            //if (Pal == null) break;
        }
        //Debug.Log("Active Touch");
        A.SetBool("Active", true);
    }

    public override void GroundLeft(Player P)
    {
        Animator A = null;
        GameObject Pal = gameObject;
        while (Pal != null)
        {
            A = Pal.GetComponent<Animator>();
            if (A == null)
            {
                if (Pal.transform.parent != null)
                    Pal = Pal.transform.parent.gameObject;
                else
                    Pal = null;
            }
            else
                break;

           // if (Pal == null) break;
        }
        Debug.Log("Active Touch");
        A.SetBool("Active", false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Animator A = gameObject.GetComponent<Animator>();
        Debug.Log("Active Touch");
        A.SetBool("Active", true);
    }

    private void OnTriggerEnter(Collider other)
    {
        Animator A = gameObject.GetComponent<Animator>();
        Debug.Log("Active Touch");
        A.SetBool("Active", true);
    }

    private void OnCollisionExit(Collision collision)
    {
        Animator A = gameObject.GetComponent<Animator>();
        Debug.Log("Dropped Active Touch");
        A.SetBool("Active", false);
    }

    private void OnTriggerExit(Collider other)
    {
        Animator A = gameObject.GetComponent<Animator>();
        Debug.Log("Dropped Active Touch");
        A.SetBool("Active", false);
    }
}
