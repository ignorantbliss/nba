using RuntimeNodeEditor;
using TMPro;
using UnityEngine;

public class ActivateNode : Node
{
    public UnityEngine.UI.Image statusField;
    public SocketInput activeSocket;    
    private int LastValue = -1;    
    IOutput Incoming;

    public ObjectTarget Targets = null;

    public override void Setup()
    {        
        Register(activeSocket);        
        SetHeader("effect/activate");

        OnConnectionEvent += OnConnection;
        OnDisconnectEvent += OnDisconnect;
    }

    public void OnConnection(SocketInput input, IOutput output)
    {        
        output.ValueUpdated += OnConnectedValueUpdated;
        Incoming = output;

        OnConnectedValueUpdated();
    }

    public bool Default = false;

    public override void ResetNode()
    {
        base.ResetNode();
        foreach (GameObject Context in Targets.Targets)
        {
            Animator A = Context.GetComponentInChildren<Animator>();
            if (A != null) A.SetBool("Active", Default);
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
        float incomingValue = Incoming.GetValue<float>();

        foreach (GameObject Context in Targets.Targets)
        {
            if (incomingValue == 0)
            {
                Animator A = Context.GetComponentInChildren<Animator>();
                if (A != null) A.SetBool("Active", Default);
            }
            else
            {
                Animator A = Context.GetComponentInChildren<Animator>();
                if (A != null) A.SetBool("Active", !Default);
            }
        }
    }
}