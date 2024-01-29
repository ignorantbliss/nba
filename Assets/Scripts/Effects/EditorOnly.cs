using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorOnly : MonoBehaviour
{
    public void StartPlayMode()
    {
        MeshRenderer[] Gs = gameObject.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer M in Gs)
        {
            M.enabled = false;
        }

        Collider[] Cs = gameObject.GetComponentsInChildren<Collider>();
        foreach (Collider C in Cs)
        {
            C.enabled = false;
        }
    }

    public void StartEditMode()
    {
        MeshRenderer[] Gs = gameObject.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer M in Gs)
        {
            M.enabled = true;
        }

        Collider[] Cs = gameObject.GetComponentsInChildren<Collider>();
        foreach (Collider C in Cs)
        {
            C.enabled = true;
        }
    }
}
