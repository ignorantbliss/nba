using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NodeUINode : MonoBehaviour
{
    public GameObject Target;

    public GameObject IncomingPoint;
    public GameObject OutgoingPoint;
    public GameObject BaseNode;

    public GameObject Content;

    bool Dragging = false;
    Vector2 DragStart = Vector2.zero;
    Vector2 DragReference = Vector2.zero;
    RectTransform RT = null;

    public void Drag(BaseEventData BDE)
    {
        Debug.Log("Dragging!");
        DragStart = Input.mousePosition;
        RT = BaseNode.GetComponent<RectTransform>();
        DragReference = RT.anchoredPosition;
        Dragging = true;
    }

    public void Drop(BaseEventData BDE)
    {
        Debug.Log("Dropping!");
        Dragging = false;
    }

    public void Hover(BaseEventData BDE)
    {
        Debug.Log("Hovering Over " + BDE.selectedObject.name);
        BDE.selectedObject.GetComponent<Image>().color = Color.yellow;
    }

    public void Unhover(BaseEventData BDE)
    {
        Debug.Log("Leaving " + BDE.selectedObject.name);
        BDE.selectedObject.GetComponent<Image>().color = Color.white;
    }

    public void Moving(BaseEventData BDE)
    {

    }

    void Refresh()
    {
        foreach (Transform T in Content.transform)
        {
            if (T.gameObject == IncomingPoint) continue;
            if (T.gameObject == OutgoingPoint) continue;
            if (T.gameObject == BaseNode) continue;
            //GameObject.Destroy(T.gameObject);
        }

        int Items = 0;
        CodeInput CI = Target.GetComponent<CodeInput>();
        if (CI != null)
        {
            foreach(string S in CI.Name)
            {
                //Debug.Log(S);
                GameObject G = GameObject.Instantiate(IncomingPoint);
                RectTransform Origin = IncomingPoint.GetComponent<RectTransform>();
                RectTransform RT = G.GetComponent<RectTransform>();
                RT.SetParent(IncomingPoint.transform.parent,false);
                RT.anchoredPosition = Origin.anchoredPosition + new Vector2(0,-20*Items);
                RT.sizeDelta = Origin.sizeDelta;
                UnityEngine.UI.Text T = G.GetComponentInChildren<Text>();
                T.text = S;
                G.SetActive(true);
                Items++;
                G.name = S;
            }
        }
        int MaxItems = Items;

        Items = 0;
        CodeOutput CO = Target.GetComponent<CodeOutput>();
        if (CO != null)
        {
            foreach (string S in CO.Outputs)
            {
                //Debug.Log(S);
                GameObject G = GameObject.Instantiate(OutgoingPoint);
                RectTransform Origin = OutgoingPoint.GetComponent<RectTransform>();
                RectTransform RT = G.GetComponent<RectTransform>();
                RT.SetParent(OutgoingPoint.transform.parent, false);
                RT.anchoredPosition = Origin.anchoredPosition + new Vector2(0, -20 * Items);
                RT.sizeDelta = Origin.sizeDelta;
                UnityEngine.UI.Text T = G.GetComponentInChildren<Text>();
                T.text = S;
                G.SetActive(true);
                Items++;
                G.name = S;
            }
        }
        if (MaxItems < Items) MaxItems = Items;

        RectTransform NodeBG = Content.transform.GetChild(0).GetComponent<RectTransform>();
        int NewHeight = (MaxItems * 20) + 10;
        Debug.Log("New Height: " + NewHeight);
        NodeBG.sizeDelta = new Vector2(NodeBG.sizeDelta.x, NewHeight);
    }

    // Start is called before the first frame update
    void Start()
    {
        Refresh();
    }

    // Update is called once per frame
    void Update()
    {
        if (Dragging == true)
        {
            RT.anchoredPosition = DragReference + ((Vector2)Input.mousePosition - DragStart);
        }
    }
}
