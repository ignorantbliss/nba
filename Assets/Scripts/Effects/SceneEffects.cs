using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SceneEffectInstance
{
    public GameObject Template;
}

public class SceneEffects : MonoBehaviour
{
    public static SceneEffects Instance = null;

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

    public GameObject DropEffect(Vector3 Position, Vector3 Normal, Vector3 Scale, string Name, string Default = "")
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
                Gx.transform.LookAt(Vector3.forward, Normal);
                Gx.transform.localScale = Scale;
                Gx.SetActive(true);
                LastEffect = Name;
                LastTime = Time.time;
                return Gx;
            }
        }
        if (Default != "")
        {
            return DropEffect(Position, Normal, Scale, Default);
        }
        return null;
    }
    
}
