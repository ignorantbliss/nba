using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Appearance : MonoBehaviour
{
    public bool OnEven = true;
    public float Speed = 4f;
    public string Material = "TickTock";    

    MeshRenderer R = null;
    Collider C = null;
    SharedValue SharedFloat = null;

    // Start is called before the first frame update
    void Start()
    {
        R = gameObject.GetComponent<MeshRenderer>();
        C = gameObject.GetComponent<Collider>();
        SharedFloat = SharedValueEngine.Get(Material,0,1);
    }

    // Update is called once per frame
    void Update()
    {
        float V = SharedFloat.Value % (Speed * 2);        
        //Debug.Log(V + " vs " + (Speed));
        if (V > Speed)
        {
            if (OnEven == false)
            {
                R.enabled = false;
                C.enabled = false;
            }
            else
            {
                R.enabled = true;
                C.enabled = true;
            }
        }
        else
        {
            if (OnEven == true)
            {
                R.enabled = false;
                C.enabled = false;
            }
            else
            {
                R.enabled = true;
                C.enabled = true;
            }
        }
    }

    void OnEnable()
    {
    }

    void OnDisable()
    {
        R.enabled = true;
        C.enabled = true;
    }
}
