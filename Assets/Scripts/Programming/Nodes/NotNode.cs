using RuntimeNodeEditor;
using TMPro;
using UnityEngine;

public class NotNode : Node
{    
    public SocketInput inSocket;
    public SocketOutput outSocket;    
    IOutput Incoming;

    public override void Setup()
    {        
        Register(inSocket);
        Register(outSocket);
        SetHeader("not");

        OnConnectionEvent += OnConnection;
        OnDisconnectEvent += OnDisconnect;
    }

    public void OnConnection(SocketInput input, IOutput output)
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
    }

    private void OnConnectedValueUpdated()
    {        
        float incomingValue = Incoming.GetValue<float>();

        if (incomingValue == 0)
        {
            outSocket.SetValue(1f);
        }
        else
        {
            outSocket.SetValue(0f);
        }
    }
}