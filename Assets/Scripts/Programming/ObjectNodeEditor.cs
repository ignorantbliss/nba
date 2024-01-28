using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeNodeEditor;
using UnityEngine.EventSystems;
using TMPro;

public class ObjectNodeEditor : NodeEditor
{
    public static NodeGraph NGraph;

    public override void StartEditor(NodeGraph G)
    {
        base.StartEditor(G);        
        Events.OnGraphPointerClickEvent += OnGraphPointerClick;        
        Events.OnNodePointerClickEvent += OnNodePointerClick;
        Events.OnConnectionPointerClickEvent += OnNodeConnectionPointerClick;
        NGraph = G;
        //Events.OnSocketConnect += OnConnect;
    }

    private void DeleteNode(Node node)
    {
        Graph.Delete(node);
        CloseContextMenu();
    }

    private void DuplicateNode(Node node)
    {
        Graph.Duplicate(node);
        CloseContextMenu();
    }

    private void DisconnectConnection(string line_id)
    {
        Graph.Disconnect(line_id);
        CloseContextMenu();
    }

    private void ClearConnections(Node node)
    {
        Graph.ClearConnectionsOf(node);
        CloseContextMenu();
    }

    private void OnNodePointerClick(Node node, PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            var ctx = new ContextMenuBuilder()
            .Add("duplicate", () => DuplicateNode(node))
            .Add("clear connections", () => ClearConnections(node))
            .Add("delete", () => DeleteNode(node))
            .Build();

            SetContextMenu(ctx);
            DisplayContextMenu();
        }
    }

    private void OnNodeConnectionPointerClick(string connId, PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            var ctx = new ContextMenuBuilder()
            .Add("clear connection", () => DisconnectConnection(connId))
            .Build();

            SetContextMenu(ctx);
            DisplayContextMenu();
        }
    }

    protected void OnGraphPointerClick(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Right:
                {
                    ContextMenuBuilder ctx = new ContextMenuBuilder()
                    .Add("Basic/Number", CreateFloatNode)
                    .Add("Timing/Tick-Tock", CreateTickTockNode)
                    .Add("Basic/Math", CreateMathNode)
                    .Add("Basic/Compare", CreateCompNode)
                    .Add("Logic/And", CreateAndNode)
                    .Add("Logic/Not", CreateNotNode)
                    .Add("Logic/Or", CreateOrNode)
                    .Add("Logic/Toggle", CreateToggleNode)
                    .Add("Logic/Latch", CreateLatchNode)                    
                    .Add("Effects/Visisble", CreateVisibleNode)
                    .Add("Effects/Operate", CreateOperateNode)                    
                    .Add("Detection/Activation", CreateActivateNode)
                    .Add("Detection/Distance", CreateDistanceNode)
                    .Add("Detection/Touched", CreateTouchedNode);

                    SetContextMenu(ctx.Build());
                    DisplayContextMenu();
                }
                break;
            case PointerEventData.InputButton.Left: CloseContextMenu(); break;
        }
    }

    void CreateFloatNode()
    {
        Graph.Create("Nodes/FloatNode");
        CloseContextMenu();
    }

    void CreateMathNode()
    {
        Graph.Create("Nodes/MathNode");
        CloseContextMenu();
    }

    void CreateCompNode()
    {
        Graph.Create("Nodes/CompareNode");
        CloseContextMenu();
    }

    void CreateDistanceNode()
    {
        Graph.Create("Nodes/DistanceNode");
        CloseContextMenu();
    }

    void CreateVisibleNode()
    {
        Graph.Create("Nodes/EffectVisible");
        //Graph.nodes[Graph.nodes.Count - 1].GetComponent<VisibleNode>().Context = PopupUI.SelectedObject;
        CloseContextMenu();
    }
    void CreateNotNode()
    {
        Graph.Create("Nodes/NotNode");        
        CloseContextMenu();
    }

    void CreateAndNode()
    {
        Graph.Create("Nodes/AndNode");
        CloseContextMenu();
    }

    void CreateOrNode()
    {
        Graph.Create("Nodes/OrNode");
        CloseContextMenu();
    }

    void CreateToggleNode()
    {
        Graph.Create("Nodes/ToggleNode");
        CloseContextMenu();
    }

    void CreateLatchNode()
    {
        Graph.Create("Nodes/LatchNode");
        CloseContextMenu();
    }

    void CreateTouchedNode()
    {
        Graph.Create("Nodes/TouchNode");
        CloseContextMenu();
    }

    void CreateTickTockNode()
    {
        Graph.Create("Nodes/TickTockNode");
        CloseContextMenu();
    }

    void CreateActivateNode()
    {
        Graph.Create("Nodes/IsActiveNode");        
        CloseContextMenu();
    }

    void CreateOperateNode()
    {
        Graph.Create("Nodes/EffectActive");
        CloseContextMenu();
    }
}


