using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoLive : MonoBehaviour
{
    public List<MonoBehaviour> ActiveInPlay = new List<MonoBehaviour>();
    public void PlayMode()
    {
        foreach (MonoBehaviour B in ActiveInPlay)
        {
            B.enabled = true;
        }
    }

    public void EditMode()
    {
        foreach (MonoBehaviour B in ActiveInPlay)
        {
            B.enabled = false;
        }
    }
}
