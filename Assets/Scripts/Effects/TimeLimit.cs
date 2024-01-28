using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLimit : MonoBehaviour
{
    public float TotalTime = 1;
    float Limit = -1;

    // Start is called before the first frame update
    void Start()
    {
        Limit = Time.time + TotalTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > Limit)
        {
            GameObject.Destroy(gameObject);
        }
    }
}
