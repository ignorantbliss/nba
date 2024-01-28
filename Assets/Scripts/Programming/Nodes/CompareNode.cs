using RuntimeNodeEditor;
using TMPro;
using UnityEngine;

public class CompareNode : Node
{
    //public UnityEngine.UI.Image statusField;
    public SocketOutput YesNo;
    public SocketInput ValueA;
    public SocketInput ValueB;
    public TMP_Dropdown Dropdown;
    public TMP_Text Result;

    private int LastValue = -1;
    public string Comparison = "=";
    IOutput NodeA;
    IOutput NodeB;

    public void CompareMode(int i)
    {
        if (i == 0) Comparison = "=";
        if (i == 1) Comparison = ">";
        if (i == 2) Comparison = "<";
    }

    public override void Setup()
    {
        Register(YesNo);
        Register(ValueA);
        Register(ValueB);
        SetHeader("comparison");

        OnConnectionEvent += OnConnection;
        OnDisconnectEvent += OnDisconnect;
    }

    public void OnConnection(SocketInput input, IOutput output)
    {
        output.ValueUpdated += OnConnectedValueUpdated;
        if (input == ValueA)
            NodeA = output;
        if (input == ValueB)
            NodeB = output;

        OnConnectedValueUpdated();
    }

    public void OnDisconnect(SocketInput input, IOutput output)
    {
        output.ValueUpdated -= OnConnectedValueUpdated;
        if (input == ValueA)
            NodeA = null;
        if (input == ValueB)
            NodeB = null;

        OnConnectedValueUpdated();
    }

    float a = 0f;
    float b = 0f;
    float YesNoValue = 0f;

    private void OnConnectedValueUpdated()
    {
        if ((NodeA == null) || (NodeB == null))
            return;

        try
        {
            a = NodeA.GetValue<float>();
            b = NodeB.GetValue<float>();

            if (Comparison == "=")
            {
                if (a == b)
                    YesNoValue = 1f;
                else
                    YesNoValue = 0f;
            }
            if (Comparison == ">")
            {
                if (a > b)
                    YesNoValue = 1f;
                else
                    YesNoValue = 0f;
            }
            if (Comparison == "<")
            {
                if (a < b)
                    YesNoValue = 1f;
                else
                    YesNoValue = 0f;
            }
        }
        catch(System.Exception E)
        {
            Debug.Log("Couldn't Do That - " + E.Message);
        }

        YesNo.SetValue(YesNoValue);

        Result.SetText(YesNoValue.ToString());
    }    
}