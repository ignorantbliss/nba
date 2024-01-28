using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedValue
{
    public float Value = 0;
    public float Rate = 0;

    public SharedValue(float V, float R)
    {
        Value = V;
        Rate = R;
    }

    public void Update()
    {
        Value += Rate * Time.deltaTime;
    }
}


public class SharedValueEngine : MonoBehaviour
{
    Dictionary<string,SharedValue> Values = new Dictionary<string,SharedValue>();

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        foreach(KeyValuePair<string,SharedValue> SV in Values)
        {
            SV.Value.Update();
        }
    }

    static SharedValueEngine Instance;

    public static SharedValue Get(string S,float StartValue = 0,float AnimateRate = 0)
    {
        if (Instance.Values.ContainsKey(S))
        {
            return Instance.Values[S];
        }
        Instance.Values.Add(S, new SharedValue(StartValue, AnimateRate));
        return Instance.Values[S];
    }
}
