using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyShader : MonoBehaviour
{
    public GameObject Target = null;
    public ShaderSets MaterialListing;
    public string MaterialName;
    public string ObjectType;
    public PopupUI Parent;

    public void ApplyChosenShader()
    {
        if (Target == null)
        {
            Parent.OpenSkinList();
        }
        else
        {
            OptionCore.Instance.gameObject.GetComponent<LandPlacement>().SetDefaultSkin(ObjectType, MaterialName);
            Debug.Log("Writing " + ObjectType + " skin " + MaterialName);
            MaterialListing.ChangeMaterialSet(ObjectType, MaterialName, Target);
            Parent.CloseSkinList();
        }
    }
}
