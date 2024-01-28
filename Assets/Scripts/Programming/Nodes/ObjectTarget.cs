using UnityEngine;
using System.Collections.Generic;

public class ObjectTarget : MonoBehaviour
{
    public TMPro.TMP_Text Counter = null;
    public List<GameObject> Targets = new List<GameObject>();

    public void SelectMembers()
    {
        EditorScene.Instance.Chooser(this);
    }

    public void UpdatedMembers()
    {
        Counter.text = Targets.Count.ToString();
    }

    public GameObject GetParent(GameObject G)
    {
        while (G.transform.parent != null)
        {
            G = G.transform.parent.gameObject;
        }
        return G;
    }
}