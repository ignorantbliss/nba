using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SceneProps : MonoBehaviour
{
    public static SceneProps Instance = null;

    public List<GameObject> EffectLibrary = new List<GameObject>();
    public string LastEffect = "";
    public float LastTime = -1;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        foreach (Transform T in gameObject.transform)
        {
            if (!EffectLibrary.Contains(T.gameObject))
            {
                EffectLibrary.Add(T.gameObject);
            }
        }
    }

    public GameObject DropProp(Vector3 Position, Vector3 Normal, Vector3 Scale, int i)
    {
        return DropProp(Position,Normal, Scale, EffectLibrary[i].name);
    }

    public GameObject DropProp(Vector3 Position, Vector3 Normal, Vector3 Scale, string Name)
    {
        if (Time.time - LastTime < 0.1f)
        {
            if (LastEffect == Name)
                return null;            
        }
        foreach(GameObject G in EffectLibrary)
        {
            if (G.name == Name)
            {
                GameObject Gx = GameObject.Instantiate(G);
                Gx.transform.position = Position;
                //Gx.transform.LookAt(Vector3.forward, Normal);
                Gx.transform.localRotation = G.transform.localRotation;
                Gx.transform.localScale = Scale;
                Gx.SetActive(true);
                LastEffect = Name;
                LastTime = Time.time;
                return Gx;
            }
        }
        return null;
    }
    
}
