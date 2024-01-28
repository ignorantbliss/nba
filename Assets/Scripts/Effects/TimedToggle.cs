using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedToggle : MonoBehaviour
{
    // Start is called before the first frame update
    public string Property;
    public float Timing;
    float TimeBase;
    int LastValue = 0;
    Animator A = null;

    void OnEnable()
    {
        TimeBase = Time.time;
    }    

    // Update is called once per frame
    void Update()
    {
        int NewValue = (int)((Time.time - TimeBase) / Timing);
        if (NewValue >= 1)
        {
            if (A == null)
            {
                A = gameObject.GetComponent<Animator>();
            }
            if (A.GetBool(Property) == true)
                A.SetBool(Property, false);
            else
                A.SetBool(Property, true);

            TimeBase = Time.time;
        }
    }
}
