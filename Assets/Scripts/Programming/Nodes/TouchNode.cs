using RuntimeNodeEditor;
using TMPro;
using UnityEngine;

public class TouchNode : Node
{        
    public SocketOutput outaSocket;
    public SocketOutput outbSocket;
    IOutput Incoming;

    public ObjectTarget Targets = null;

    public override void Setup()
    {                
        Register(outaSocket);
        Register(outbSocket);
        SetHeader("Touched");

        //OnConnectionEvent += OnConnection;
        //OnDisconnectEvent += OnDisconnect;
    }

    /*public void OnConnection(SocketInput input, IOutput output)
    {        
        output.ValueUpdated += OnConnectedValueUpdated;
        Incoming = output;

        OnConnectedValueUpdated();
    }

    public void OnDisconnect(SocketInput input, IOutput output)
    {        
        output.ValueUpdated -= OnConnectedValueUpdated;
        Incoming = null;

        OnConnectedValueUpdated();
    }*/

    public void PlayerTouch()
    {
        outaSocket.SetValue(1);
    }

    public override void ResetNode()
    {
        //Add collision detectors on chosen objects...
        foreach (GameObject G in Targets.Targets)
        {
            foreach (Toucher Tx in G.GetComponents<Toucher>())
            {
                if (Tx.Node == this)
                {
                    continue;
                }
            }
            Toucher T = G.AddComponent<Toucher>();
            T.Node = this;
        }        
        base.ResetNode();
    }    
    
}