using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetOption : MonoBehaviour
{
    public int Offset = 0;

    public void SetString(string Val)
    {
        OptionCore.Instance.ChangeOptionString(gameObject.name, Val);
    }

    public void SetInt(int Val)
    {
        OptionCore.Instance.ChangeOptionInt(gameObject.name, Val + Offset);
    }
}
