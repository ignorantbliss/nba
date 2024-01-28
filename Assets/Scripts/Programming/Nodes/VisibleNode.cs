using RuntimeNodeEditor;
using TMPro;
using UnityEngine;

public class VisibleNode : Node
{
    public UnityEngine.UI.Image statusField;
    public SocketInput activeSocket;    
    private int LastValue = -1;    
    IOutput Incoming;

    public ObjectTarget Targets = null;

    public override void Setup()
    {        
        Register(activeSocket);        
        SetHeader("effect/visible");

        OnConnectionEvent += OnConnection;
        OnDisconnectEvent += OnDisconnect;
    }

    public void OnConnection(SocketInput input, IOutput output)
    {        
        output.ValueUpdated += OnConnectedValueUpdated;
        Incoming = output;

        OnConnectedValueUpdated();
    }

    public override void ResetNode()
    {
        base.ResetNode();
        foreach (GameObject Context in Targets.Targets)
        {
            Context.GetComponent<MeshRenderer>().enabled = true;
            try
            {
                Context.GetComponent<Collider>().enabled = true;
            }
            catch
            {

            }
        }
    }

    public void OnDisconnect(SocketInput input, IOutput output)
    {        
        output.ValueUpdated -= OnConnectedValueUpdated;
        Incoming = null;

        OnConnectedValueUpdated();
    }

    private void OnConnectedValueUpdated()
    {
        Debug.Log("Updating...");
        float incomingValue = Incoming.GetValue<float>();

        foreach (GameObject Context in Targets.Targets)
        {
            if (incomingValue == 0)
            {
                Debug.Log("Making Invisible");
                Context.GetComponent<MeshRenderer>().enabled = false;
                try
                {
                    Context.GetComponent<Collider>().enabled = false;
                }
                catch
                {

                }
            }
            else
            {
                Debug.Log("Making Visible");
                Context.GetComponent<MeshRenderer>().enabled = true;
                try
                {
                    Context.GetComponent<Collider>().enabled = true;
                }
                catch
                {

                }
            }
        }
    }
}