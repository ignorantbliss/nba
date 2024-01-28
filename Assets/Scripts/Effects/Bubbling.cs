using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bubbling : MonoBehaviour
{
    public int MaterialIndex = 1;    
    public float Speed = 0.1f;
    public string Material = "Lava";
    public Vector2 Direction = new Vector2(1, 0);

    MeshRenderer R = null;
    SharedValue SharedFloat = null;

    // Start is called before the first frame update
    void Start()
    {
        R = gameObject.GetComponent<MeshRenderer>();
        SharedFloat = SharedValueEngine.Get(Material,0,Speed);
    }

    // Update is called once per frame
    void Update()
    {        
        R.materials[MaterialIndex].mainTextureOffset = SharedFloat.Value * Direction;
    }
}
