using RuntimeNodeEditor;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DistanceNode : Node
{    
    public SocketOutput activeSocket;

    public ObjectTarget Targets = null;

    GameObject PlayerObject = null;

    public override void Setup()
    {
        activeSocket.SetValue(9999);
        Register(activeSocket); 
        //SetHeader("Player Distance");
    }
    
    void Update()
    {
        if (PlayerObject == null)
        {
            PlayerObject = PlayerControl.Instance.Player;
            //PlayerObject = 
        }
        float Dist = 99999;
        foreach(GameObject G in Targets.Targets)
        {
            float D = (G.transform.position - PlayerObject.transform.position).magnitude;
            if (D < Dist)
            {
                Dist = D;
            }
        }

        //Debug.Log("Outputting: " + Dist);
        
        activeSocket.SetValue(Dist);
    }
}