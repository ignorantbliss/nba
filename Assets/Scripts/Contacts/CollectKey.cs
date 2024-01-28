using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectKey : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    public void PlayerTouch(Player P)
    {
        Debug.Log("Coin Touched!");
        Animator A = GetComponent<Animator>();
        if (A == null)
        {
            if (transform.parent != null)
            {
                A = transform.parent.gameObject.GetComponent<Animator>();
            }
        }
        if (A != null) A.SetBool("Triggered", true);
        SceneEffects.Instance.DropEffect(transform.position, Vector3.up, Vector3.one, "Coin");
        GetComponent<Collider>().enabled = false;

    }
}
