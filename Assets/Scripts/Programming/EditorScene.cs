using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeNodeEditor;

public class EditorScene : MonoBehaviour
{
    public RectTransform Inside;
    public ObjectNodeEditor Editor;
    protected NodeGraph graph;

    public static int Mode = 0;
    public static EditorScene Instance;

    void Start()
    {
        Instance = this;
        Debug.Log("Starting Node Editor...");
        gameObject.SetActive(true);
        graph = Editor.CreateGraph<NodeGraph>(Inside);
        //graph.SetSize(new Vector2(1000, 1000));
        Editor.StartEditor(graph);

        RectTransform[] GOs = gameObject.GetComponentsInChildren<RectTransform>();
        foreach (RectTransform T in GOs)
        {
            if (T.gameObject.name == "NodeGraph")
            {
                T.localScale = Vector3.one;
            }
        }
    }

    public void ResetAll()
    {
        foreach (Node N in graph.nodes)
        {
            N.ResetNode();
        }
    }

    public void OpenEditor()
    {
        /*gameObject.SetActive(true);
        var graph = Editor.CreateGraph<NodeGraph>(Inside);
        Editor.StartEditor(graph); */
    }

    public void Toggle()
    {
        if (Mode == 2)
        {
            Mode = 1;
            Animator A = gameObject.GetComponent<Animator>();
            A.SetBool("Visible", true);
        }
        else
        {
            Animator A = gameObject.GetComponent<Animator>();
            A.SetBool("Visible", !A.GetBool("Visible"));
            if (A.GetBool("Visible") == true)
                Mode = 1;
            else
                Mode = 0;
        }
    }

    public ObjectTarget SettingTargets = null;

    public void Chooser(ObjectTarget T)
    {
        Animator A = gameObject.GetComponent<Animator>();
        A.SetBool("Visible", false);
        Mode = 3;
        SettingTargets = T;
    }

    public void ConfirmSelection(GameObject G)
    {
        Animator A = gameObject.GetComponent<Animator>();
        if (!SettingTargets.Targets.Contains(G))
            SettingTargets.Targets.Add(G);
        Mode = 1;
        A.SetBool("Visible", true);
        SettingTargets.UpdatedMembers();

    }

    public void AddSelection(GameObject G)
    {        
        if (!SettingTargets.Targets.Contains(G))
            SettingTargets.Targets.Add(G);
        SettingTargets.UpdatedMembers();
    }

    public void RemoveSelection(GameObject G)
    {
        if (SettingTargets.Targets.Contains(G))
            SettingTargets.Targets.Remove(G);
        SettingTargets.UpdatedMembers();
    }

    public void Restart()
    {
        graph.Trigger();
    }
}
