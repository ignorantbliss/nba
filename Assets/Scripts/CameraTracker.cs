using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracker : MonoBehaviour
{
    public GameObject ToTrack = null;
    public float Distance = 8;
    public Vector3 Angle = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        Angle = -transform.forward;
        //Angle = (transform.position - ToTrack.transform.position).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position,ToTrack.transform.position + (Angle * Distance),0.1f);
    }
}
