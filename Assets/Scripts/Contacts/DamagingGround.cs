using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagingGround : Grounded
{
    public override void GroundHit(Player P)
    {
        Debug.Log("Damager Touched!");
        Animator A = GetComponent<Animator>();
        if (A == null)
        {
            if (transform.parent != null)
            {
                A = transform.parent.gameObject.GetComponent<Animator>();
            }
        }
        if (A != null) A.SetBool("Triggered", true);
        SceneEffects.Instance.DropEffect(transform.position, Vector3.up, Vector3.one, "Ooof");
        P.Hit(1);
        P.PushAway(3);
    }
}
