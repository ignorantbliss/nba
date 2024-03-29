using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupUI : MonoBehaviour
{
    public GameObject Targ;

    public UnityEngine.UI.Text Title;
    public RectTransform SkinButton;
    public ShaderSets MaterialListing;
    public OpenHelp Help;

    public UnityEngine.UI.Dropdown SkinDrop = null;
    public static GameObject SelectedObject = null;

    List<GameObject> SkinButtons = null;

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void ApplySkin(int i)
    {
        string NewName = null;
        int indx = -1;
        ShaderSets.ShaderSet[] Sets = MaterialListing.GetMaterialsFor(Targ.name);

        MeshRenderer MR = Targ.GetComponent<MeshRenderer>();
        //Material M = MR.sharedMaterial;

        Debug.Log("Applying Different Skin...");

        Material[] Mats = new Material[MR.sharedMaterials.Length];

        foreach (ShaderSets.ShaderSet S in Sets)
        {
            if (S.ApplyToNo == 0)
            {
                indx++;
                if (indx == i)
                {
                    NewName = S.MaterialName;
                }                
            }

            if (NewName == S.MaterialName)
            {
                Mats[S.ApplyToNo] = S.M;// MR.sharedMaterials[S.ApplyToNo] = S.M;
                //Debug.Log("Applying Skin " + S.MaterialName + " - " + S.M.name + " To " + Targ.name + " Layer " + S.ApplyToNo);
            }
        }

        MR.sharedMaterials = Mats;
    }

    public void Open(GameObject Target)
    {
        Targ = Target;
        gameObject.SetActive(true);

        UpdateSkinTexture();        

        Title.text = Targ.name;

        Help.Subject = Title.text;
        SelectedObject = Target;
    }

    public void Delete()
    {
        //if (Title.text != "StartSpot")
        //{
            GameObject.Destroy(Targ);
            Close();
        //}
    }

    public void UpdateSkinTexture()
    {
        ShaderSets.ShaderSet[] Sets = MaterialListing.GetMaterialsFor(Targ.name);

        if (Sets.Length == 0)
        {
            SkinDrop.gameObject.SetActive(false);
            //SkinButton.gameObject.SetActive(false);
            return;
        }
        else
        {
            SkinDrop.gameObject.SetActive(true);
            //SkinButton.gameObject.SetActive(true);
        }

        SkinDrop.ClearOptions();
        List<string> OptionList = new List<string>();
        foreach (ShaderSets.ShaderSet S in Sets)
        {
            if (S.ApplyToNo == 0)
                OptionList.Add(S.MaterialName);
        }
        SkinDrop.AddOptions(OptionList);

        MeshRenderer MR = Targ.GetComponent<MeshRenderer>();
        Material M = MR.sharedMaterial;
        UnityEngine.UI.RawImage Img = SkinButton.GetComponentInChildren<UnityEngine.UI.RawImage>();        
        if (M.HasProperty("_MainTex"))
        {
            Texture T = Sets[0].M.GetTexture("_MainTex");
            if (T != null)
                Img.texture = T;
            else
                Img.color = M.GetColor("_Color");
        }
        else
        {
            Img.color = M.GetColor("_Color");
        }
        
    }

    public EditorScene NEditor;
    

    public void OpenEditor()
    {
        NEditor.OpenEditor();
    }

    public void CloseSkinList()
    {
        if (SkinButtons != null)
        {
            foreach (GameObject G in SkinButtons)
            {
                GameObject.Destroy(G);
            }
        }
        SkinButtons = new List<GameObject>();
    }

    public void OpenSkinList()
    {
        /*if (SkinButtons != null)
        {
            foreach(GameObject G in SkinButtons)
            {
                GameObject.Destroy(G);
            }
        }
        SkinButtons = new List<GameObject>();

        Debug.Log("Getting Skins for " + Targ.name);
        ShaderSets.ShaderSet[] Sets = MaterialListing.GetMaterialsFor(Targ.name);

        List<string> ShaderName = new List<string>();

        int Counter = 0;
        for (int x = 0; x < Sets.Length; x++)
        {            
            //Debug.Log("Found " + Sets[x].MaterialName);

            if (ShaderName.Contains(Sets[x].MaterialName))
            {
                continue;
            }
            ShaderName.Add(Sets[x].MaterialName);
            Counter = Counter + 1;
            
            GameObject GO = GameObject.Instantiate(SkinButton.gameObject);           
            RectTransform RT = GO.GetComponent<RectTransform>();
            RT.SetParent(SkinButton.parent);
            GO.name = Sets[x].MaterialName;
            RT.anchorMax = SkinButton.anchorMax;
            RT.anchorMin = SkinButton.anchorMin;
            RT.anchoredPosition = SkinButton.anchoredPosition + new Vector2(0, -((Counter+1) * 60));
            SkinButtons.Add(GO);            

            ApplyShader AS = GO.GetComponent<ApplyShader>();
            AS.Target = Targ;
            AS.ObjectType = Targ.name;
            AS.MaterialName = Sets[x].MaterialName;

            UnityEngine.UI.RawImage RI = GO.GetComponentInChildren<UnityEngine.UI.RawImage>();
            if (Sets[x].M.HasProperty("_MainTex"))
            {
                Texture T = Sets[x].M.GetTexture("_MainTex");
                if (T != null)
                    RI.texture = T;
                else
                {
                    RI.color = Sets[x].M.GetColor("_Color");
                }
                //Debug.Log("Using Texture from Material");
            }
            else
            {
                RI.color = Sets[x].M.GetColor("_Color");
            }

            
            foreach(Transform G in GO.transform)
            {
                UnityEngine.UI.Text Txt = G.gameObject.GetComponent<UnityEngine.UI.Text>();
                if (Txt != null)
                {
                    Txt.text = AS.MaterialName;
                    Txt.gameObject.SetActive(true);
                }
            }            
        }*/

    }
}
