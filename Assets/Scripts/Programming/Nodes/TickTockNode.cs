using RuntimeNodeEditor;
using TMPro;
using UnityEngine;

public class TickTockNode : Node
{
    public UnityEngine.UI.Image statusField;
    public SocketOutput tickSocket;    
    private int LastValue = -1;

    public override void Setup()
    {
        Register(tickSocket);        
        SetHeader("tick/tock");
    }

    void Update()
    {
        if (PlayerControl.Instance != null)
        {            
            if (PlayerControl.Instance.TickTock)
            {
                if (LastValue != 1)
                {
                    tickSocket.SetValue(1f);                    
                    statusField.color = UnityEngine.Color.green;
                    LastValue = 1;
                    Debug.Log("Ticking");
                }
            }
            else
            {
                if (LastValue != 0)
                {
                    tickSocket.SetValue(0f);                    
                    statusField.color = UnityEngine.Color.red;
                    LastValue = 0;
                    Debug.Log("Tocking");
                }
            }
        }
    }
}