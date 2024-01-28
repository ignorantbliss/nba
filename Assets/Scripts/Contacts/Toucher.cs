using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toucher : MonoBehaviour
{

    // Start is called before the first frame update

    public TouchNode Node = null;
    
    public void PlayerTouch(Player P)
    {
        Node.PlayerTouch();

    }
}
