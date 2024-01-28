using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    public Vector3 Amount;

    // Update is called once per frame
    void Update()
    {
        transform.localRotation *= Quaternion.Euler(Amount * Time.deltaTime);
    }
}
