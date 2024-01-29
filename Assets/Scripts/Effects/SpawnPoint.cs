using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public void StartPlayMode()
    {
        if (enabled == true)
        {
            PlayerControl.Instance.Player.transform.position = transform.position + (transform.forward * 1);
        }
    }

    public void StartEditMode()
    {
        PlayerControl.Instance.Player.transform.position = transform.position + (transform.forward * 1);
    }
}
