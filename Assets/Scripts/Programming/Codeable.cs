using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Codeable : MonoBehaviour
{
    public class Connection
    {
        public GameObject InputObject;
        public string InputName;

        public GameObject OutputObject;
        public string OutputName;

        public GameObject UILink = null;
    }

    public List<Connection> Connections = new List<Connection>();

}
