using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShaderSets : MonoBehaviour
{

    [Serializable]
    public class ShaderSet
    {
        public string Object = "";
        public string MaterialName = "";
        public int ApplyToNo = 0;        
        public Material M = null;
    }

    public List<ShaderSet> Materials = new List<ShaderSet>();
    
    public void ChangeMaterialSet(string ObjectType, string Name, GameObject GO)
    {
        foreach (ShaderSet S in Materials)
        {
            if (S.Object == ObjectType)
            {
                if (S.MaterialName == Name)
                {
                    MeshRenderer Ren = GO.GetComponent<MeshRenderer>();

                    Material[] Current = Ren.sharedMaterials;
                    Material[] Lst = new Material[Ren.sharedMaterials.Length];
                    int x = -1;
                    foreach (Material M in Ren.sharedMaterials)
                    {
                        x++;
                        Lst[x] = Current[x];
                        if (x == S.ApplyToNo)
                        {
                            Lst[x] = S.M;
                        }
                    }
                    Ren.sharedMaterials = Lst;

                    ApplySkinSettings(ObjectType, Name, GO);

                }
            }
        }
    }

    public static void ApplySkinSettings(string ObjectType, string Name,GameObject GO)
    {
        if (ObjectType != Name)
        {
            SkinName SN = GO.GetComponent<SkinName>();
            if (SN == null)
            {
                SN = GO.AddComponent<SkinName>();
            }
            SN.Name = Name;
        }
        if (ObjectType == "Vanishing")
        {
            if (Name == "Blue")
            {
                Appearance A = GO.GetComponent<Appearance>();
                if (A != null)
                {
                    A.OnEven = false;
                }
            }
            else
            {
                Appearance A = GO.GetComponent<Appearance>();
                if (A != null)
                {
                    A.OnEven = true;
                }
            }
        }
        if (ObjectType == "Teleporter")
        {
            TeleportingGround G = GO.GetComponent<TeleportingGround>();
            switch (Name)
            {
                case "Blue":
                    G.GroupNumber = 0;
                    break;
                case "Red":
                    G.GroupNumber = 1;
                    break;
                case "Green":
                    G.GroupNumber = 2;
                    break;
            }
        }
    }

    public ShaderSet[] GetMaterialsFor(string ObjectType)
    {
        List<ShaderSet> Set = new List<ShaderSet>();
        foreach (ShaderSet S in Materials)
        {
            if (S.Object == ObjectType)
            {
                Set.Add(S);
            }
        }
        return Set.ToArray();
    }
}
