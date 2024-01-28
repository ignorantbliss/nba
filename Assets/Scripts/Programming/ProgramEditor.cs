using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ProgramEditor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Mode == DragMode.existingitem)
        {
            if (!Input.GetMouseButton(0))
            {
                Mode = DragMode.none;
                Dropping(null);
            }
        }
    }

    public List<GameObject> CodePieces = new List<GameObject>();

    GameObject DraggingObject = null;
    PointerEventData PED = null;
    RectTransform RT = null;

    int DropIndex = -1;
    enum DragMode { newitem, existingitem, none };
    DragMode Mode = DragMode.newitem;

    public void StartedDragging(BaseEventData ED)
    {
        PED = (PointerEventData)ED;
        Debug.Log("Drag Start for " + PED.pointerDrag.name);

        if (PED.pointerDrag.transform.parent.name == "ProgramPanel")
        {
            DraggingObject = PED.pointerDrag;
            Image I = DraggingObject.GetComponentInChildren<Image>();
            Color C = new Color(I.color.r, I.color.g, I.color.b, 0.25f);
            RT = DraggingObject.GetComponent<RectTransform>();
            RectTransform Original = PED.pointerDrag.GetComponent<RectTransform>();
            RT.SetParent(Original.parent, false);
            RT.position = Original.position;
            RT.anchoredPosition = Original.anchoredPosition;
            CodePieces.Remove(RT.gameObject);
            FormatPieces();
            Mode = DragMode.existingitem;
        }
        else
        {
            DraggingObject = GameObject.Instantiate(PED.pointerDrag);
            Image I = DraggingObject.GetComponentInChildren<Image>();
            Color C = new Color(I.color.r, I.color.g, I.color.b, 0.25f);
            RT = DraggingObject.GetComponent<RectTransform>();
            RectTransform Original = PED.pointerDrag.GetComponent<RectTransform>();
            RT.SetParent(Original.parent, false);
            RT.position = Original.position;
            RT.anchoredPosition = Original.anchoredPosition;
            Mode = DragMode.newitem;
        }
        
    }

    bool ValidDrop = false;

    public void Dragging(BaseEventData ED)
    {
        PED = (PointerEventData)ED;
        RT.transform.position += (Vector3)PED.delta;
        ValidDrop = false;
        if (PED.pointerCurrentRaycast.gameObject != null)
        {
            //Debug.Log("Drag Continues for " + ((PointerEventData)ED).pointerCurrentRaycast.gameObject.name);
            if (PED.pointerCurrentRaycast.gameObject.name == "ProgramPanel")
            {
                ValidDrop = true;
                DropIndex = 0;
            }
            if (Mode == DragMode.newitem)
            {
                RT.SetParent(PED.pointerCurrentRaycast.gameObject.GetComponent<RectTransform>(), true);
            }
            
        }        

    }

    void FormatPieces()
    {
        float ypos = -15;
        float buffer = -10;
        foreach(GameObject G in CodePieces)
        {
            RectTransform Rct = G.GetComponent<RectTransform>();
            Rct.anchoredPosition = new Vector2(10, ypos);
            ypos -= Rct.sizeDelta.y + buffer;
        }
    }

    public void Dropping(BaseEventData ED)
    {
        Debug.Log("Dropping for " + RT.transform.name);
        if (!CodePieces.Contains(RT.gameObject))
        {
            CodePieces.Add(RT.gameObject);
        }
        FormatPieces();
    }
}
