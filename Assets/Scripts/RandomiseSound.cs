using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomiseSound : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Src.volume = (Src.volume - VRange) + (Random.value * VRange);
        Src.pitch = (Src.pitch - PRange) + (Random.value * PRange);
    }

    public AudioSource Src;
    public float VRange = 0.4f;
    public float PRange = 0.2f;
}
