using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Xml;
using UnityEngine.Networking;

public class LandPlacement : MonoBehaviour
{
    public int LandLayer = 2;
    public int GridLayer = 3;
    public int PropLayer = 4;
    public GameObject EditorPlane = null;
    public GameObject LandPrefab = null;

    public List<GameObject> Prefabs = new List<GameObject>();

    GameObject Draft = null;
    int PrefabNo = 0;
    int PropNo = 0;

    enum DraftStyle { land, prop, select, none };
    DraftStyle DraftType = DraftStyle.land;
    DraftStyle PlaceMode = DraftStyle.land;
    Quaternion PropRotation = Quaternion.identity;

    void OnDisable()
    {
        PlayerUI.SetActive(true);
        PlayerUI.GetComponent<PlayerUIManager>().WakeUp();
    }

    void OnEnable()
    {
        PlayerUI.SetActive(false);        
    }

    // Start is called before the first frame update
    void Start()
    {
        /*string filename = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "\\Save.xml";
        if (File.Exists(filename))
        {
            LoadGame(filename);
        }*/
    }

    public UnityEngine.UI.Text ModeName;

    public void SetTerrain()
    {
        PlaceMode = DraftStyle.land;
        ModeName.text = "Terrain Mode";
    }    

    public void SetProp()
    {
        PlaceMode = DraftStyle.prop;
        ModeName.text = "Detail Mode";
    }

    public void NextMode()
    {
        if (PlaceMode == DraftStyle.land)
        {
            SetProp();
        }
        else
        {
            SetTerrain();
        }
    }

    public GameObject PlayerUI;

    public void SetMaterial(int i)
    {
        PrefabNo = i;
        if (Draft != null)
        {
            GameObject.Destroy(Draft);
            Draft = null;
        }
        SetTerrain();
    }

    float RotationAmount = 0;

    public void SetProp(int i)
    {
        PropNo = i;
        if (Draft != null)
        {
            GameObject.Destroy(Draft);
            Draft = null;
        }
        SetProp();
    }

    bool HoldingMouse = false;
    Vector3 HoldingMouseRef = Vector3.zero;
    Vector3 HoldingMouseRefLast = Vector3.zero;
    GameObject HoldingMouseObj = null;
    int HoldingScaleOffset = 0;
    int HoldingPositionOffset = 0;
    float PropSizeFactor = 1;
    //float ScalingSize = 60;

    public string ToSaveContent = "";
    public string ToSaveName = "";

    public PopupUI Popup = null;

    // Update is called once per frame

    public UnityEngine.UI.InputField XMLSaveControl;

    public void ToggleClip()
    {
        if (XMLSaveControl.gameObject.activeSelf == false)
        {
            XMLSaveControl.gameObject.SetActive(true);
            XMLSaveControl.text = GetSaveContent();
        }
        else
        {
            string LevelText = XMLSaveControl.text;
            LoadGame(LevelText,true);
            XMLSaveControl.gameObject.SetActive(false);
        }
    }

    public string GetSaveContent()
    {
        string skin = "";
        SkinName SN = null;
        StringBuilder SB = new StringBuilder("");
        SB.Append("<level>\r\n");
        GameObject[] Ground = GameObject.FindGameObjectsWithTag("Terrain");
        foreach (GameObject G in Ground)
        {
            float y = 1;
            try
            {
                Bounds b = G.GetComponent<MeshRenderer>().bounds;
                Debug.Log("Object Bounds: " + b.size.y);
                y = (int)(b.size.y / 0.25f);
            }
            catch
            {

            }
            skin = G.name;
            SN = G.GetComponent<SkinName>();
            if (SN != null)
            {
                skin = SN.Name;
            }
            SB.Append("  <chunk type=\"" + G.name + "\" x=\"" + G.transform.position.x + "\" y=\"" + G.transform.position.z + "\" height=\"" + y + "\" skin=\"" + skin + "\" floor=\"0\"/>\r\n");
        }

        Ground = GameObject.FindGameObjectsWithTag("Decoration");
        foreach (GameObject G in Ground)
        {
            SB.Append("  <decoration type=\"" + G.name + "\" x=\"" + G.transform.position.x + "\" y=\"" + G.transform.position.z + "\" z=\"" + G.transform.position.y + "\" skin=\"" + G.name + "\" />\r\n");
        }
        RuntimeNodeEditor.GraphData GD = ObjectNodeEditor.NGraph.Export();
        foreach (RuntimeNodeEditor.NodeData ND in GD.nodes)
        {
            SB.Append("  <node type=\"" + ND.path + "\" x=\"" + ND.posX + "\" y=\"" + ND.posY + "\" id=\"" + ND.id + "\">");
            foreach (RuntimeNodeEditor.SerializedValue NV in ND.values)
            {
                SB.Append("    <value name=\"" + NV.key + "\" value=\"" + NV.value + "\"/>");
            }
            foreach (string S in ND.inputSocketIds)
            {
                SB.Append("    <input>" + S + "</input>");
            }
            foreach (string S in ND.outputSocketIds)
            {
                SB.Append("    <output>" + S + "</output>");
            }
            SB.Append("  </node>");
        }

        foreach (RuntimeNodeEditor.ConnectionData CD in GD.connections)
        {
            SB.Append("   <link from=\"" + CD.inputSocketId + "\" to=\"" + CD.outputSocketId + "\"/>");
        }
        SB.Append("</level>");

        return SB.ToString();
    }

    public void SaveGame()
    {
        if (GameName.text.Trim() != "")
        {
            string content = GetSaveContent();
            

#if !UNITY_WEBGL

        string filename = "";
        filename = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "\\NoisyBigAdventure\\" + GameName.text + ".xml";

        TextWriter Wrtr = (StreamWriter)File.CreateText(filename);
        Wrtr.Write(content);
        Wrtr.Flush();
        Wrtr.Close();
#else


#endif
            ToSaveContent = content;
            ToSaveName = GameName.text.ToString();

            StartCoroutine(SaveToServer());
        }
    }

    public IEnumerator SaveToServer()
    {
        WWWForm frm = new WWWForm();
        frm.AddField("name", ToSaveName);
        frm.AddField("content", ToSaveContent);

        using (UnityWebRequest www = UnityWebRequest.Post("https://connos.info/nbe/save.php", frm))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.downloadHandler.text);
                ToggleSaveUI();
            }
            else
            {
                Debug.Log("Failure.");
                Debug.Log(www.downloadHandler.text);
                ToggleSaveUI();
            }
        }
    }

    public GameObject SaveGameUI;
    public UnityEngine.UI.InputField GameName;
    public GameObject SaveGameEntry;

    public void ToggleSaveUI()
    {
        if (SaveGameUI.activeSelf == false)
        {
            XMLSaveControl.text = GetSaveContent();
            foreach (Transform T in SaveGameUI.transform)
            {
                T.gameObject.SetActive(true);
            }
            //ReloadSaveUI();
            RefreshSaves();
            SaveGameUI.SetActive(true);
        }
        else
        {
            //SaveGameUI.SetActive(false);
        }
    }

    List<SaveGameInfo> SaveGames = new List<SaveGameInfo>();

    void RefreshSaves()
    {
        StartCoroutine(GetSavegames());
    }

    IEnumerator GetSavegames()
    {
        Transform based = SaveGameEntry.transform.parent;
        for (int x = based.childCount - 1; x >= 0; x--)
        {
            GameObject G = based.GetChild(x).gameObject;
            if (G == SaveGameEntry) continue;
            GameObject.Destroy(G);
        }
        
        WWWForm frm = new WWWForm();
        frm.AddField("mode", "top10");

        using (UnityWebRequest www = UnityWebRequest.Post("https://connos.info/nbe/list.php", frm))
        {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log(www.downloadHandler.text);

                XmlDocument Doc = new XmlDocument();
                Doc.LoadXml(www.downloadHandler.text);
                XmlNodeList NL = Doc.GetElementsByTagName("venture");
                SaveGames = new List<SaveGameInfo>();
                foreach (XmlNode N in NL)
                {
                    SaveGames.Add(new SaveGameInfo(N.InnerText, int.Parse(N.Attributes["id"].Value)));
                }
            }
            else
            {
                Debug.LogError("Failed to load saved games - " + www.result);
            }
        }
        Debug.Log("Loaded " + SaveGames.Count + " savegames.");
        ReloadSaveUI();
    }

    IEnumerator GetSavegame(int id)
    {
        SaveGames.Clear();
        WWWForm frm = new WWWForm();
        frm.AddField("id", id.ToString());

        using (UnityWebRequest www = UnityWebRequest.Post("https://connos.info/nbe/load.php", frm))
        {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Loaded Save Game From Server");
                LoadGame(www.downloadHandler.text,true);
            }
            else
            {
                Debug.LogError("Failed to load saved games - " + www.result);
            }
        }        
    }

    void ReloadSaveUI()
    {
        Transform based = SaveGameEntry.transform.parent;
        for(int x=based.childCount-1;x>=0;x--)
        {
            GameObject G = based.GetChild(x).gameObject;
            if (G == SaveGameEntry) continue;
            GameObject.Destroy(G);
        }

        RectTransform RTX = SaveGameEntry.GetComponent<RectTransform>();

        int fcount = 0;
        //string[] Files = Directory.GetFiles(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "\\NoisyBigAdventure");
        //foreach (string F in Files)
        foreach(SaveGameInfo SI in SaveGames)
        {            
            //string FName = Path.GetFileName(F);
            //if (Path.GetExtension(F) == ".xml")
            //{                
                GameObject G = GameObject.Instantiate(SaveGameEntry);
                G.GetComponentInChildren<UnityEngine.UI.Text>().text = SI.Name;
                G.transform.SetParent(SaveGameEntry.transform.parent, true);
                RectTransform RT = G.GetComponent<RectTransform>();
                RT.anchoredPosition = RTX.anchoredPosition - new Vector2(0, 25*fcount);
                RT.sizeDelta = RTX.sizeDelta;// * 0.85f;
                G.SetActive(true);
                LoadGameButton LGB = G.GetComponent<LoadGameButton>();
                LGB.SaveGame = SI.ID.ToString();

                fcount++;
           // }

        }
    }

    public void DeleteLevel()
    {
        GameObject[] Objs = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject G in Objs)
        {
            if (G.layer == 10)
            {
                if (G.name != "StartSpot")
                {
                    GameObject.Destroy(G);
                }
            }
            if (G.layer == 12)
            {
                GameObject.Destroy(G);
            }
        }
    }

    public void LoadSavegame(string nm)
    {
        Debug.Log("Loading Savegame " + nm);
        foreach (SaveGameInfo SV in SaveGames)
        {
            Debug.Log("Comparing " + SV.ID + " to " + nm);
            if (SV.ID.ToString() == nm)
            {
                Debug.Log("Found ID: " + SV.ID.ToString());
                StartCoroutine(GetSavegame(SV.ID));
                return;
            }
        }
    }

    public void LoadGame(string filename,bool XML = false)
    {
        DeleteLevel();

        string Content = "";
        if (XML == false)
        {
            filename = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "\\NoisyBigAdventure\\" + filename;
            TextReader Rdr = (TextReader)File.OpenText(filename);
            Content = Rdr.ReadToEnd();
            Rdr.Close();
        }
        else
        {
            Content = filename;
        }

        Debug.Log("Loading " + Content);

        XmlDocument Doc = new XmlDocument();
        Doc.LoadXml(Content);

        XmlNodeList Chunks = Doc.GetElementsByTagName("chunk");
        foreach (XmlNode Nd in Chunks)
        {
            //Generate Chunks...

            int indx = -1;
            foreach (GameObject G in Prefabs)
            {
                indx++;
                if (G.name == Nd.Attributes["type"].Value)
                {
                    PrefabNo = indx;
                }
            }

            Draft = GameObject.Instantiate(Prefabs[PrefabNo]);
            Draft.name = Draft.name.Replace("(Clone)", "");
            Draft.SetActive(true);
            Draft.GetComponent<Collider>().enabled = false;
            Draft.tag = "Terrain";
            DraftType = DraftStyle.land;
            Draft.transform.position = new Vector3(float.Parse(Nd.Attributes["x"].Value),0, float.Parse(Nd.Attributes["y"].Value));
            Draft.GetComponentInChildren<Collider>().enabled = true;
            SetSize(Draft, int.Parse(Nd.Attributes["height"].Value));

            //string skn = GetDefaultSkin(Prefabs[PrefabNo].name);
            //Debug.Log("Default Skin: " + skn);
            string skn = Nd.Attributes["skin"].Value;
            if (skn != Prefabs[PrefabNo].name)
            {                
                ShaderSets MaterialListing = gameObject.GetComponent<ShaderSets>();
                MaterialListing.ChangeMaterialSet(Prefabs[PrefabNo].name, skn, Draft);
            }

            if (XML == false)
            {
                string NewTitle = Path.GetFileNameWithoutExtension(filename);
                GameName.text = NewTitle;
            }
            ToggleSaveUI();

            Draft = null;
        }
    }

    public Material SelectionMaterial;

    Dictionary<GameObject, GameObject> SelectedIndicators = new Dictionary<GameObject, GameObject>();

    void UpdateSelectionIndicators()
    {        
        foreach (GameObject G in EditorScene.Instance.SettingTargets.Targets)
        {
            if (!SelectedIndicators.ContainsKey(G))
            {
                GameObject O = GameObject.Instantiate(G);
                O.transform.localScale = O.transform.localScale * 1.1f;
                MeshRenderer[] Renderers = O.GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer R in Renderers)
                {
                    R.sharedMaterial = SelectionMaterial;
                }
                SelectionUI UI = O.AddComponent<SelectionUI>();
                UI.Original = G;
                SelectedIndicators[G] = O;
            }
        }

        List<GameObject> ToRemove = new List<GameObject>();
        foreach (GameObject K in SelectedIndicators.Keys)
        {
            if (!EditorScene.Instance.SettingTargets.Targets.Contains(K))
            {
                ToRemove.Add(K);
            }
        }

        foreach (GameObject K in ToRemove)
        {
            try
            {
                GameObject.Destroy(SelectedIndicators[K]);
            }
            catch
            {
            }

            SelectedIndicators.Remove(K);
        }
    }

    void ClearSelections()
    {
        Debug.Log("Clearing All Selection UIs");
        foreach (KeyValuePair<GameObject, GameObject> KV in SelectedIndicators)
        {
            Debug.Log("Destroying Selection...");
            GameObject.Destroy(KV.Value);
        }
        SelectedIndicators.Clear();
    }

    void Update()
    {
        if (EditorScene.Mode != 2)
        {
            if (SelectedIndicators.Count > 0)
            {
                ClearSelections();
            }            
        }
        if (EditorScene.Mode == 1)
        {
            return;
        }
        if (EditorScene.Mode == 2)
        {
            if ((SelectedIndicators.Count == 0) && (EditorScene.Instance.SettingTargets.Targets.Count > 0))
            {
                UpdateSelectionIndicators();
                PlaceMode = DraftStyle.select;
                DraftType = DraftStyle.none;
                ModeName.text = "Selection Mode";
            }            
        }
        if (EventSystem.current.IsPointerOverGameObject())
        {
            if (Draft != null)
            {
                GameObject.Destroy(Draft);
                Draft = null;
            }
            return;
        }
        if (Input.GetKeyDown(KeyCode.Insert))
        {
            PrefabNo++;
            if (PrefabNo >= Prefabs.Count)
            {
                PrefabNo = 0;
            }
            Debug.Log("Switched To " + Prefabs[PrefabNo].name);
            if (Draft != null)
            {
                GameObject.Destroy(Draft);
                Draft = null;
            }
        }

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            PrefabNo--;
            if (PrefabNo < 0)
            {
                PrefabNo = Prefabs.Count-1;
            }
            Debug.Log("Switched To " + Prefabs[PrefabNo].name);
            if (Draft != null)
            {
                GameObject.Destroy(Draft);
                Draft = null;
            }
        }

        if (HoldingMouse == true)
        {
            if (Input.GetMouseButton(0))
            {
                if (Input.mousePosition != HoldingMouseRefLast)
                {
                    HoldingMouseRefLast = Input.mousePosition;
                    float Diff = (HoldingMouseRefLast.y - HoldingMouseRef.y);
                    float S = Diff / 60;
                    int Steps;
                    if (S > 0)
                        Steps = Mathf.CeilToInt(Diff / 60);
                    else
                        Steps = Mathf.CeilToInt(Diff / 60);

                    if (Input.GetMouseButton(1))
                    {
                        HoldingMouseObj.transform.position = new Vector3(HoldingMouseObj.transform.position.x, HoldingPositionOffset + (Steps * 0.1f) + HoldingPositionOffset, HoldingMouseObj.transform.position.z);
                    }
                    else
                    {

                        if (Steps + HoldingScaleOffset == 0) Steps = 1;
                        if (Steps + HoldingScaleOffset < 0) Steps = 1 - HoldingScaleOffset;
                        SetSize(HoldingMouseObj, Steps + HoldingScaleOffset);
                    }
                }
            }
            else
            {
                HoldingMouse = false;
            }
            return;
        }

        Ray R = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit H = new RaycastHit();        
        RaycastHit[] Hits = Physics.RaycastAll(R, 1000);
        float Dist = 1000;
        float D = 0;

        foreach (RaycastHit Ht in Hits)
        {
            D = (Camera.main.transform.position - Ht.point).sqrMagnitude;            
            if (Ht.collider.gameObject.layer == PropLayer)
            {
                if (D < Dist)
                {
                    Dist = D;
                    H = Ht;                 
                }
            }
            if (Ht.collider.gameObject.layer == LandLayer)
            {
                if (D < Dist)
                {
                    Dist = D;
                    H = Ht;
                }
            }
            if (Ht.collider.gameObject.layer == GridLayer)
            {
                if (D < Dist)
                {
                    Dist = D;
                    H = Ht;
                }
            }            
        }
        //if (H.collider != null)
        //    Debug.Log("Contact With " + H.collider.gameObject.name);
        if (H.collider != null)
        {
            if (EditorScene.Mode == 2)
            {
                //Add this item to the context list...
                if (H.collider.gameObject.layer != GridLayer)
                {
                    Debug.Log("Selection: " + H.collider.gameObject.name);
                    if (Input.GetMouseButtonDown(0))
                    {
                        SelectionUI SUI = H.collider.gameObject.GetComponent<SelectionUI>();
                        if (SUI != null)
                        {
                            EditorScene.Instance.RemoveSelection(SUI.Original);
                            GameObject.Destroy(H.collider.gameObject);
                        }
                        else
                        {
                            if (Input.GetKey(KeyCode.LeftShift))
                            {
                                EditorScene.Instance.AddSelection(H.collider.gameObject);
                            }
                            else
                            {
                                EditorScene.Instance.ConfirmSelection(H.collider.gameObject);
                            }
                        }
                        UpdateSelectionIndicators();
                    }
                }
            }
            if (EditorScene.Mode == 3)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    EditorScene.Mode = 2;
                    return;
                }

                if (Draft != null)
                {
                    GameObject.Destroy(Draft);
                    Draft = null;
                }

                if (H.collider.gameObject.layer == PropLayer)
                {
                    //Highlighting Object
                    if (Highlighted != null)
                    {
                        if (Highlighted != H.collider.gameObject)
                        {
                            UnHighlight();
                        }

                        Highlight(H.collider.gameObject);
                    }
                    else
                        Highlight(H.collider.gameObject);

                    if (Input.GetMouseButtonDown(0))
                    {
                        //This should do it...
                        Debug.LogWarning("Selecting " + H.collider.gameObject.name);
                        EditorScene.Instance.ConfirmSelection(Highlighted);
                        UnHighlight();
                    }

                }
            }

            if ((H.collider.gameObject.layer == PropLayer))
            {
                if (Input.GetMouseButtonDown(1))
                {
                    if (PlaceMode == DraftStyle.prop)
                    {
                        Transform Tx = H.collider.gameObject.transform;
                        while ((Tx.parent != null) && (Tx.parent.gameObject.layer == PropLayer))
                        {
                            Tx = Tx.parent;
                        }
                        Popup.Open(Tx.gameObject);
                        //GameObject.Destroy(Tx.gameObject);
                    }
                }
            }
            if ((H.collider.gameObject.layer == LandLayer))
            {
                EditorPlane.transform.position = H.collider.gameObject.transform.position;
                if (Input.GetMouseButtonDown(1))
                {
                    if (HoldingMouse == false)
                    {
                        if (PlaceMode == DraftStyle.land)
                        {
                            Popup.Open(H.collider.gameObject);
                            //GameObject.Destroy(H.collider.gameObject);
                        }
                    }
                }
                
                if (Input.GetKeyDown(KeyCode.U))
                {
                    Enlarge(H.collider.gameObject);
                }
                if (Input.GetKeyDown(KeyCode.J))
                {
                    Shrink(H.collider.gameObject);
                }
                if (Input.GetKeyDown(KeyCode.I))
                {
                    MoveUp(H.collider.gameObject);
                }
                if (Input.GetKeyDown(KeyCode.K))
                {
                    MoveDown(H.collider.gameObject);
                }                
                if (Input.GetMouseButtonDown(0))
                {
                    HoldingMouse = true;
                    HoldingMouseRef = Input.mousePosition;
                    HoldingMouseObj = H.collider.gameObject;
                    HoldingPositionOffset = (int)Mathf.Ceil(HoldingMouseObj.transform.position.y * 10);
                    HoldingScaleOffset = GetCurrentSize(HoldingMouseObj);
                }
                if (HoldingMouse == true)
                {
                    if (!Input.GetMouseButton(0))
                        HoldingMouse = false;
                }

                if ((PlaceMode == DraftStyle.prop) && (PropNo != -1))
                {
                    if (Draft != null)
                    {
                        if (DraftType == DraftStyle.land)
                        {
                            GameObject.Destroy(Draft);
                            Draft = null;
                        };
                    }

                    if ((H.normal == Vector3.up) && (EditorScene.Mode != 3))
                    {

                        //Check for an existing prop in this position...                     

                        Vector3 Pos = H.point;
                        if (Pos.x > 0)
                            Pos.x += 0.5f;
                        if (Pos.x < 0)
                            Pos.x -= 0.5f;

                        if (Pos.z > 0)
                            Pos.z += 0.5f;
                        if (Pos.z < 0)
                            Pos.z -= 0.5f;

                        Pos = new Vector3((int)(Pos.x / PropSizeFactor), Pos.y, (int)(Pos.z / PropSizeFactor));
                        bool Valid = true;

                        RaycastHit Hx = new RaycastHit();
                        if (!Physics.Raycast(Pos + (Vector3.up * 5), -Vector3.up, out Hx, 100))
                        {
                            //Debug.Log("Clear");
                        }
                        else
                        {
                            //Debug.Log("Not Clear - " + Hx.collider.name + " @ " + Hx.point + " / " + Hx.normal + " / " + Hx.collider.gameObject.layer);
                            if (Hx.collider.gameObject.layer == PropLayer)
                            {                                
                                Valid = false;
                                if (Draft != null)
                                {
                                    Collider[] Colls = Draft.GetComponentsInChildren<Collider>();
                                    foreach (Collider C in Colls)
                                    {
                                        if (C == Hx.collider)
                                            Valid = true;
                                        break;
                                    }
                                }
                            }
                        }
                        if (Valid == true)
                        { 
                            if (Draft == null)
                            {
                                Draft = SceneProps.Instance.DropProp(H.point, H.normal, Vector3.one, PropNo);
                                Draft.name = Draft.name.Replace("(Clone)","");
                                DraftType = DraftStyle.prop;
                                PropRotation = Draft.transform.localRotation;
                                PlayerControl.Instance.CurrentGameMode = PlayerControl.GameMode.prop;
                            }

                            //Pos = new Vector3(Mathf.Ceil(Pos.x),Pos.y, Mathf.Ceil(Pos.z));
                            //Debug.Log(H.point + " / " + Pos);

                            Draft.transform.position = Pos;
                            Draft.transform.localRotation = PropRotation;

                            RotationAmount += Input.mouseScrollDelta.y * 5f;

                            Draft.transform.Rotate(new Vector3(0, RotationAmount, 0));
                            //Debug.Log("Nothing Hit @ " + (Pos + (Vector3.up * 5)));
                        }
                        else
                        {
                            Debug.Log("Something Was Hit - " + Hx.collider.name);
                            if (Draft != null)
                            {
                                GameObject.Destroy(Draft);
                                Draft = null;
                            }
                        }

                    }
                   
                }
                else
                {
                    if (Draft != null)
                    {
                        GameObject.Destroy(Draft);
                        Draft = null;
                    }
                }

            }
            if (H.collider.gameObject.layer == GridLayer)
            {
                if (EditorScene.Mode == 2)
                {
                    return;
                }
                if (PlaceMode == DraftStyle.land)
                {
                    if (Draft != null)
                    {
                        if (DraftType == DraftStyle.prop)
                        {
                            GameObject.Destroy(Draft);
                            Draft = null;
                        };
                    }
                    if (Draft == null)
                    {
                        Draft = GameObject.Instantiate(Prefabs[PrefabNo]);
                        Draft.name = Draft.name.Replace("(Clone)", "");
                        Draft.SetActive(true);
                        Draft.GetComponent<Collider>().enabled = false;
                        Draft.tag = "Terrain";
                        DraftType = DraftStyle.land;
                        PlayerControl.Instance.CurrentGameMode = PlayerControl.GameMode.terrain;
                        string skn = GetDefaultSkin(Prefabs[PrefabNo].name);
                        //Debug.Log("Default Skin: " + skn);
                        if (skn != Prefabs[PrefabNo].name)
                        {
                            //Debug.Log("Default Skin: " + skn);
                            ShaderSets MaterialListing = gameObject.GetComponent<ShaderSets>();
                            MaterialListing.ChangeMaterialSet(Prefabs[PrefabNo].name, skn, Draft);
                        }

                    }
                    Vector3 Pos = H.point;
                    if (Pos.x > 0)
                        Pos.x += 0.5f;
                    if (Pos.x < 0)
                        Pos.x -= 0.5f;

                    if (Pos.z > 0)
                        Pos.z += 0.5f;
                    if (Pos.z < 0)
                        Pos.z -= 0.5f;

                    Pos = new Vector3((int)(Pos.x / 1), Pos.y, (int)(Pos.z / 1));

                    //Pos = new Vector3(Mathf.Ceil(Pos.x),Pos.y, Mathf.Ceil(Pos.z));
                    //Debug.Log(H.point + " / " + Pos);
                    if (Pos == H.collider.gameObject.transform.position)
                    {
                        GameObject.Destroy(Draft);
                    }
                    else
                    {
                        Draft.transform.position = Pos;
                    }
                }
                else
                {
                    if (Draft != null)
                    {
                        GameObject.Destroy(Draft);
                        Draft = null;
                    }
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (Draft != null)
                {
                    Draft.GetComponentInChildren<Collider>().enabled = true;                    
                    string Sound = Prefabs[PrefabNo].name;                    
                    SceneEffects.Instance.DropEffect(H.point, H.normal, Vector3.one, Sound,"Grass");
                    if (PlaceMode == DraftStyle.land)
                    {
                        HoldingMouse = true;
                        HoldingMouseRef = Input.mousePosition;
                        HoldingMouseObj = Draft;
                        HoldingPositionOffset = (int)Mathf.Ceil(HoldingMouseObj.transform.position.y * 10);
                        HoldingScaleOffset = GetCurrentSize(HoldingMouseObj);
                    }

                    Draft = null;
                }
            }
        }
    }

    public Material HighlightMaterial;
    GameObject Highlighted = null;
    Dictionary<MeshRenderer, Material> BackupMaterials = new Dictionary<MeshRenderer, Material>();

    void Highlight(GameObject G)
    {
        //Assign New Materials...
        try
        {
            MeshRenderer[] Rens = G.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer MR in Rens)
            {
                if (!BackupMaterials.ContainsKey(MR))
                {
                    BackupMaterials.Add(MR, MR.sharedMaterial);
                }
                MR.sharedMaterial = HighlightMaterial;
            }
            MeshRenderer Ren = G.GetComponent<MeshRenderer>();
            if (!BackupMaterials.ContainsKey(Ren))
            {
                if (!BackupMaterials.ContainsKey(Ren))
                {
                    BackupMaterials.Add(Ren, Ren.sharedMaterial);
                }
                Ren.sharedMaterial = HighlightMaterial;
            }
        }
        catch(System.Exception e)
        {
            Debug.LogError("Failed To Apply Material - " + e.Message);
        }
        Highlighted = G;
    }

    void UnHighlight()
    {
        //Restore Original Materials...
        foreach (KeyValuePair<MeshRenderer,Material> KV in BackupMaterials)
        {
            KV.Key.sharedMaterial = KV.Value;
        }

        BackupMaterials.Clear();
        Highlighted = null;
    }

    Dictionary<int, Mesh> Meshes = new Dictionary<int, Mesh>();
    Dictionary<int, float> Sizes = new Dictionary<int, float>();

    int GetCurrentSize(GameObject G)
    {
        //float Ht = G.GetComponent<MeshFilter>().sharedMesh.bounds.size.z;
        //float Tg = Meshes[1].bounds.size.z;        
        Mesh M = G.GetComponent<MeshFilter>().sharedMesh;
        foreach (KeyValuePair<int, Mesh> KV in Meshes)
        {
            if (M == KV.Value)
            {
                return KV.Key;
            }
        }

        return 1;
    }
    
    void SetSize(GameObject G, int Sz)
    {        
        if (Meshes.Count == 0)
        {
            Meshes.Add(1, G.GetComponent<MeshFilter>().sharedMesh);
        }

        //int Sz = GetCurrentSize(G);
        //Sz++;

        if (Meshes.ContainsKey(Sz))
        {
            G.GetComponent<MeshFilter>().sharedMesh = Meshes[Sz];
            Bounds B = Meshes[Sz].bounds;
            BoxCollider BC = G.GetComponent<BoxCollider>();
            BC.center = B.center;
            BC.size = B.size * 1.1f;
        }
        else
        {
            Mesh BaseMesh = Meshes[1];
            float BaseHeight = Meshes[1].bounds.size.z;
            float HalfBase = BaseHeight / 2;

            Mesh M = new Mesh();
            M.name = "Size Level " + Sz;
            Vector3[] Verts = Meshes[1].vertices;
            Vector2[] UVs = BaseMesh.uv;
            for (int x = 0; x < Verts.Length; x++)
            {
                if (Verts[x].z > HalfBase)
                {
                    Verts[x].z += (BaseHeight * (Sz - 1));
                    //UVs[x].z += (UVs[x].z * (Sz));
                }
            }

            //Debug.Log("Old Mesh Has " + BaseMesh.subMeshCount + " sub meshes!");

            M.vertices = Verts;
            M.subMeshCount = BaseMesh.subMeshCount;
            for (int x = 0; x < BaseMesh.subMeshCount; x++)
            {
                int[] Tris = BaseMesh.GetTriangles(x);
                M.SetTriangles(Tris, x);
                //Debug.Log("Setting Sub-Mesh " + x);
            }

            M.uv = UVs;

            //Debug.Log("New Mesh Has " + M.subMeshCount + " sub meshes!");

            M.RecalculateBounds();
            M.RecalculateNormals();
            M.RecalculateTangents();
            Meshes.Add(Sz, M);
            Sizes.Add(Sz, M.bounds.size.z);
            G.GetComponent<MeshFilter>().sharedMesh = M;
            Bounds B = M.bounds;
            BoxCollider BC = G.GetComponent<BoxCollider>();
            BC.center = B.center;
            BC.size = B.size;
        }
        AdjustProps(G);
    }

    void Enlarge(GameObject G)
    {        
        if (Meshes.Count == 0)
        {
            Meshes.Add(1, G.GetComponent<MeshFilter>().sharedMesh);
        }

        int Sz = GetCurrentSize(G);
        Sz++;        

        if (Meshes.ContainsKey(Sz))
        {            
            G.GetComponent<MeshFilter>().sharedMesh = Meshes[Sz];
            Bounds B = Meshes[Sz].bounds;
            BoxCollider BC = G.GetComponent<BoxCollider>();
            BC.center = B.center;
            BC.size = B.size;
        }
        else
        {
            Mesh BaseMesh = Meshes[1];
            float BaseHeight = Meshes[1].bounds.size.z;
            float HalfBase = BaseHeight / 2;

            Mesh M = new Mesh();
            M.name = "Size Level " + Sz;
            Vector3[] Verts = Meshes[1].vertices;
            Vector2[] UVs = BaseMesh.uv;
            for (int x = 0; x < Verts.Length; x++)
            {
                if (Verts[x].z > HalfBase)
                {
                    Verts[x].z += (BaseHeight * (Sz - 1));
                    //UVs[x].z += (UVs[x].z * (Sz));
                }
            }

            Debug.Log("Old Mesh Has " + BaseMesh.subMeshCount + " sub meshes!");

            M.vertices = Verts;
            M.subMeshCount = BaseMesh.subMeshCount;
            for(int x=0;x<BaseMesh.subMeshCount;x++)
            {
                int[] Tris = BaseMesh.GetTriangles(x);
                M.SetTriangles(Tris,x);
                Debug.Log("Setting Sub-Mesh " + x);
            }            

            M.uv = UVs;

            Debug.Log("New Mesh Has " + M.subMeshCount + " sub meshes!");

            M.RecalculateBounds();
            M.RecalculateNormals();
            M.RecalculateTangents();
            Meshes.Add(Sz, M);
            Sizes.Add(Sz, M.bounds.size.z);
            G.GetComponent<MeshFilter>().sharedMesh = M;
            Bounds B = M.bounds;
            BoxCollider BC = G.GetComponent<BoxCollider>();
            BC.center = B.center;
            BC.size =B.size;
        }

        AdjustProps(G);
    }

    void Shrink(GameObject G)
    {
        int Sz = GetCurrentSize(G);
        Sz--;
        if (Sz >= 1)
        {
            G.GetComponent<MeshFilter>().sharedMesh = Meshes[Sz];
            Bounds B = Meshes[Sz].bounds;
            BoxCollider BC = G.GetComponent<BoxCollider>();
            BC.center = B.center;
            BC.size = B.size;
        }

        AdjustProps(G);
    }

    void AdjustProps(GameObject G)
    {
        //Debug.Log("Adjusting Props On This Object");
        //Search all gameobjects around this area...
        GameObject[] Gs = GameObject.FindGameObjectsWithTag("Prop");
        BoxCollider BC = G.GetComponent<BoxCollider>();
        Vector3 refpos = new Vector3(G.transform.position.x, 0,G.transform.position.z);

        Debug.Log(BC.center + " / " + BC.size);
        foreach (GameObject Gx in Gs)
        {
            if ((new Vector3(Gx.transform.position.x,0,Gx.transform.position.z) - refpos).magnitude < 0.5f)
            {
                //Apply new height...                
                Gx.transform.position = new Vector3(Gx.transform.position.x,BC.center.z + (BC.size.z / 2),Gx.transform.position.z);
                //Debug.Log("Tweaking Position Of " + Gx.name + " to " + Gx.transform.position);
            }
        }
    }

    void MoveUp(GameObject G)
    {
        G.transform.position += Vector3.up * 0.1f;
    }

    void MoveDown(GameObject G)
    {
        G.transform.position += -Vector3.up * 0.1f;
    }

    Dictionary<string, string> DefaultSkin = new Dictionary<string, string>();

    public string GetDefaultSkin(string s)
    {
        if (DefaultSkin.ContainsKey(s))
            return DefaultSkin[s];
        return s;
    }

    public void SetDefaultSkin(string s,string v)
    {
        if (s == v)
        {
            if (DefaultSkin.ContainsKey(s))
            {
                DefaultSkin.Remove(s);
            }
        }
        else
        {
            DefaultSkin[s] = v;
        }
    }
}

class SaveGameInfo
{
    public SaveGameInfo(string n, int i)
    {
        Name = n;
        ID = i;
    }
    public string Name;
    public int ID;
}