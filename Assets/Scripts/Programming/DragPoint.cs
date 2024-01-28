using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragPoint : MonoBehaviour
{
    // Start is called before the first frame update
    public UnityEngine.UI.Text TextObject;
    public UnityEngine.UI.Image ImageObject;
    public FullNodeEditor EditorBase;
    public bool Input = false;

    public void Hover()
    {
        if (EditorBase.DragNode == this)
        {
            TextObject.color = Color.red;
            ImageObject.color = Color.red;
            return;
        }
        if (EditorBase.DragNode != null)
        {
            if (EditorBase.DragNode.Input == Input)
            {
                TextObject.color = Color.grey;
                ImageObject.color = Color.grey;
                return;
            }
            else
            {
                TextObject.color = Color.green;
                ImageObject.color = Color.green;
                return;
            }
        }
        TextObject.color = Color.yellow;
        ImageObject.color = Color.yellow;
    }

    public void Leave()
    {
        if ((EditorBase.DragNode == null) || (EditorBase.DragNode != this))
        {
            TextObject.color = Color.white;
            ImageObject.color = Color.white;
        }
    }

    public void MouseDown()
    {
        if (EditorBase.DragNode == this)
        {
            TextObject.color = Color.white;
            ImageObject.color = Color.white;
            EditorBase.DragNode = null;
            return;
        }
        else
        {
            if (EditorBase.DragNode != null)
            {
                if (EditorBase.DragNode.Input != Input)
                {
                    EditorBase.ConnectNodes(this, EditorBase.DragNode);
                    return;
                }
            }
        }
        TextObject.color = Color.red;
        ImageObject.color = Color.red;
        EditorBase.DragNode = this;
    }

    public void MouseUp()
    {

    }
}
