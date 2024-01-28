using RuntimeNodeEditor;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IsActiveNode : Node
{
    public UnityEngine.UI.Image statusField;
    public SocketOutput activeSocket;            

    public ObjectTarget Targets = null;
    public List<Animator> Animators = new List<Animator>();

    public override void Setup()
    {        
        Register(activeSocket);        
        SetHeader("activated");
    }
    
    void Update()
    {        
        if (Targets.Targets.Count != Animators.Count)
        {
            //Reload Animators
            Animators.Clear();

            foreach (GameObject G in Targets.Targets)
            {
                GameObject Gx = Targets.GetParent(G);
                Animators.Add(Gx.GetComponent<Animator>());
            }
        }

       

        foreach (Animator A in Animators)
        {
            try
            {
                if (A.GetBool("Active") == true)
                {
                    activeSocket.SetValue(1f);
                    return;
                }
            }
            catch
            {
            }
            try
            {
                if (A.GetBool("Triggered") == true)
                {
                    activeSocket.SetValue(1f);
                    return;
                }
            }
            catch
            {
            }

        }

        activeSocket.SetValue(0f);
    }
}