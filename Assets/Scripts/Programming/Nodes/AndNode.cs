using RuntimeNodeEditor;
using TMPro;
using UnityEngine;

public class AndNode : Node
{    
    public SocketInput aSocket;
	public SocketInput bSocket;
    public SocketOutput outSocket;    
    IOutput AO;
    IOutput BO;

    public override void Setup()
    {        
        Register(aSocket);
		Register(bSocket);
        Register(outSocket);
        SetHeader("and");

        OnConnectionEvent += OnConnection;
        OnDisconnectEvent += OnDisconnect;
    }

    public void OnConnection(SocketInput input, IOutput output)
    {
        if (input == aSocket)
        {
            AO = output;
        }
        if (input == bSocket)
        {
            BO = output;
        }
        output.ValueUpdated += OnConnectedValueUpdated;        

        OnConnectedValueUpdated();
    }

    public void OnDisconnect(SocketInput input, IOutput output)
    {        
        output.ValueUpdated -= OnConnectedValueUpdated;
        if (input == aSocket)
        {
            AO = null;
        }
        if (input == bSocket)
        {
            BO = null;
        }

        OnConnectedValueUpdated();
    }

    private void OnConnectedValueUpdated()
    {
        try
        {
            float a = AO.GetValue<float>();
            float b = BO.GetValue<float>();

            if ((a == 1) && (b == 1))
            {
                outSocket.SetValue(1f);
            }
            else
            {
                outSocket.SetValue(0f);
            }
        }
        catch
        {

        }
    }
}