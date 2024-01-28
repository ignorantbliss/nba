using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureChanges : MonoBehaviour
{
    public List<Material> Materials = new List<Material>();

    public void SetMaterials(int x)
    {
        MeshRenderer MR = gameObject.GetComponent<MeshRenderer>();
        MR.sharedMaterial = Materials[x];
    }
}
