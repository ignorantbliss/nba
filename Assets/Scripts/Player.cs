using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject LifeMeter;
    public int Lives = 5;
    public float InvulnerableTime = 2;
    public float Coins = 0;

    Color OrigColour = Color.white;

    float InvTime = -1;
    // Start is called before the first frame update
    void Start()
    {
        OrigColour = gameObject.GetComponent<MeshRenderer>().sharedMaterial.color;
    }

    public void Revive()
    {
        transform.parent.gameObject.GetComponentInChildren<Animator>().SetBool("Dead", false);
        gameObject.GetComponent<MeshRenderer>().sharedMaterial.color = OrigColour;
        UpdateHearts(5);
    }

    public void Restart()
    {
        Debug.Log("Restarting the Player");
        Revive();
        GetComponentInChildren<Collider>().enabled = true;
    }

    public void Stop()
    {
        Debug.Log("Stopping the Player");
        GetComponentInChildren<Collider>().enabled = false;
    }

    public void AddCoin(int count=1)
    {
        Coins += count;
    }

    public void DropCoin(int count = 1)
    {
        Coins -= count;
    }

    public void Kill()
    {
        SceneEffects.Instance.DropEffect(transform.position, Vector3.up, Vector3.one, "Ooof");
        transform.parent.gameObject.GetComponent<Animator>().SetBool("Dead", true);
        UpdateHearts(0);
    }

    public void Hit(int points = 1)
    {
        if (InvTime > -1)
            return;

        Lives--;
        if (Lives <= 0)
        {
            Kill();
        }
        else
        {
            UpdateHearts(Lives);
            InvTime = Time.time + InvulnerableTime;
        }
        OrigColour = gameObject.GetComponent<MeshRenderer>().sharedMaterial.color;
        gameObject.GetComponent<MeshRenderer>().sharedMaterial.color = Color.red;
    }

    public void PushAway(float Amount)
    {
        Velocity = -ImpactDirection * Amount;
        Debug.Log("Pulse Velocity: " + Velocity);
    }

    public Vector3 ImpactPoint;
    public Vector3 ImpactDirection;
    public Vector3 Velocity = Vector3.zero;

    void UpdateHearts(int x)
    {
        string code = "H" + x.ToString();
        foreach(Transform t in LifeMeter.transform)
        {
            if (t.gameObject.name.CompareTo(code) > 0)
            {
                t.gameObject.GetComponent<Animator>().SetBool("Active", false);
                t.gameObject.SetActive(true);
            }
            else
            {
                t.gameObject.GetComponent<Animator>().SetBool("Active", true);
                t.gameObject.SetActive(true);
            }
        }
        Lives = x;
    }

    void RestoreHearts(int x)
    {
        string code = "H" + x.ToString();
        foreach (Transform t in LifeMeter.transform)
        {            
            t.gameObject.GetComponent<Animator>().SetBool("Active", true);            
        }
    }

    void OnTriggerEnter(Collider other)
    {
        ImpactPoint = other.ClosestPoint(transform.position);
        ImpactDirection = (other.gameObject.transform.position - gameObject.transform.position).normalized;
        Debug.Log("Touching Player..." + other.gameObject.name);
        other.gameObject.SendMessageUpwards("PlayerTouch",this);        
    }

    void OnTriggerExit(Collider other)
    {        
    }

    void OnCollisionEnter(Collision other)
    {
        Debug.Log("Striking Player..." + other.gameObject.name);
        other.gameObject.SendMessageUpwards("PlayerTouch", this);
    }

    void OnCollisionExit(Collision other)
    {
    }

    void Update()
    {
        if (InvTime != -1)
        {
            if (InvTime < Time.time)
            {
                InvTime = -1;
                gameObject.GetComponent<MeshRenderer>().sharedMaterial.color = OrigColour;
            }
        }
    }

    public void Teleport(Vector3 V)
    {
        PlayerControl.Instance.Teleport(V + (transform.up*2));
    }
}
