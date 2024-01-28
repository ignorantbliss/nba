using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeOutput : MonoBehaviour
{
    public List<string> Outputs;
    Dictionary<string, GameObject> InputLinks = new Dictionary<string, GameObject>();

    public void OnTriggerOutput(string Name)
    {
        if (InputLinks.ContainsKey(Name))
        {
            InputLinks[Name].SendMessage(Name, this);
        }
    }

    public void Connect(string Output, GameObject Input)
    {
        InputLinks.Add(Output, Input);
    }
}
