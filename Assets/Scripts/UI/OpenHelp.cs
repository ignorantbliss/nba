using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenHelp : MonoBehaviour
{
    public string Subject = "";

    public void OpenURL()
    {
        Application.OpenURL("http://connos.info/index.php/Nbe:" + Subject.ToLower());

    }
}
