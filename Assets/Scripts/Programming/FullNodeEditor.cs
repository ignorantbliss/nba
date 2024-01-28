using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullNodeEditor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (DefaultSelection != null)
        {
            PrimaryObject(DefaultSelection);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject Game = null;
    public UnityEngine.UI.Dropdown AddDrop;
    public UnityEngine.UI.Dropdown SelectDrop;
    public GameObject Templates;

    public GameObject LinkLine = null;

    public GameObject Node = null;
    public GameObject DefaultSelection = null;

    public void PrimaryObject(GameObject G)
    {
        Debug.Log("Refreshing Editor Around " + G.name);
        GameObject NewNode = GameObject.Instantiate(Node);
        RectTransform Original = Node.GetComponent<RectTransform>();
        RectTransform NN = NewNode.GetComponent<RectTransform>();
        NN.SetParent(Original.parent, true);
        NN.anchoredPosition = new Vector3(-320, -40);        
        NodeUINode UI = NewNode.GetComponent<NodeUINode>();
        UI.Target = G;
        UI.enabled = true;
        NewNode.SetActive(true);
        NewNode.name = G.name;
        NewNode.GetComponent<NodeUINode>().enabled = true;

        UnityEngine.UI.Text[] Texts = NewNode.GetComponentsInChildren<UnityEngine.UI.Text>();
        foreach (UnityEngine.UI.Text T in Texts)
        {
            if (T.gameObject.name == "HeadingText")
            {
                T.text = G.name;
                break;
            }
        }
    }

    List<Codeable.Connection> Connections = new List<Codeable.Connection>();

    public void ConnectNodes(DragPoint A, DragPoint B)
    {
        Debug.Log("Connecting Nodes " + A.name + " to " + B.name);

        Codeable.Connection Cx = null;
        foreach (Codeable.Connection C in Connections)
        {
            if (C.InputName == A.name)
            {
                if (C.OutputName == B.name)
                {
                    Cx = C;
                    break;
                }
            }
        }

        if (Cx != null)
        {
            GameObject.Destroy(Cx.UILink);
            Cx.UILink = null;
            Connections.Remove(Cx);

            DragNode = null;
            A.Leave();
            B.Leave();
            return;
        }

        DragNode = null;
        A.Leave();
        B.Leave();
        GameObject G = GameObject.Instantiate(LinkLine);        
        RectTransform RT = G.GetComponent<RectTransform>();
        RectTransform OR = LinkLine.GetComponent<RectTransform>();
        RT.SetParent(OR.parent, true);
        RT.anchoredPosition = OR.anchoredPosition;
        RT.sizeDelta = OR.sizeDelta;

        UILine UIL = G.GetComponent<UILine>();
        UIL.PointA = A.gameObject.GetComponentInChildren<Image>().GetComponent<RectTransform>();
        UIL.PointB = B.gameObject.GetComponentInChildren<Image>().GetComponent<RectTransform>();
        G.SetActive(true);

        Codeable.Connection CC = new Codeable.Connection();
        if (A.Input == true)
        {
            CC.InputName = A.name;
            CC.OutputName = B.name;
        }
        else
        {
            CC.InputName = B.name;
            CC.OutputName = A.name;
        }
        CC.UILink = UIL.gameObject;
        Connections.Add(CC);
    }

    public DragPoint DragNode = null;

    public void SecondaryObject(GameObject G)
    {
        Debug.Log("Adding Secondary Node: " + G.name);
        GameObject NewNode = GameObject.Instantiate(Node);
        RectTransform Original = Node.GetComponent<RectTransform>();
        RectTransform NN = NewNode.GetComponent<RectTransform>();
        NN.SetParent(Original.parent, true);
        NN.anchoredPosition = new Vector3(-720, -40);
        NodeUINode UI = NewNode.GetComponent<NodeUINode>();
        UI.Target = G;
        UI.enabled = true;
        NewNode.SetActive(true);
        NewNode.name = G.name;

        UnityEngine.UI.Text[] Texts = NewNode.GetComponentsInChildren<UnityEngine.UI.Text>();
        foreach (UnityEngine.UI.Text T in Texts)
        {
            if (T.gameObject.name == "HeadingText")
            {
                T.text = G.name;
                break;
            }
        }
    }

    public void AddNew(int itemid)
    {
        string Name = AddDrop.options[itemid].text;
        foreach (Transform T in Templates.transform)
        {
            if (T.gameObject.name == Name)
            {
                GameObject Ob = GameObject.Instantiate(T.gameObject);
                Ob.SetActive(true);
                Ob.transform.SetParent(Game.transform, false);
                Ob.name = Name + " 1";
                SecondaryObject(Ob);
                break;
            }
        }
        AddDrop.value = 0;        
    }

    public void SelectExisting(int itemid)
    {
        string Name = SelectDrop.options[itemid].text;
        Codeable[] Coded = GameObject.FindObjectsOfType<Codeable>();
        foreach (Codeable C in Coded)
        {
            if (C.gameObject.name == Name)
            {
                SecondaryObject(C.gameObject);
            }
        }
    }    

}
