using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportingGround : Grounded
{
    public int GroupNumber = 0;
    public float CooldownTimer = -1;
    public Color BaseColour = Color.cyan;
    public AudioClip TPSound = null;

    private Renderer R = null;

    void Start()
    {
        R = gameObject.GetComponent<Renderer>();
        BaseColour = R.material.GetColor("_Color");
    }

    void Update()
    {
        if (CooldownTimer > -1)
        {
            CooldownTimer -= Time.deltaTime;
            if (CooldownTimer < 0)
                CooldownTimer = -1;

            float Perc = CooldownTimer / 5;
            gameObject.GetComponent<Renderer>().materials[0].SetColor("_Color", Color.Lerp(BaseColour, Color.black, Perc));
        }
    }

    public override void GroundHit(Player P)
    {
        //Debug.Log("It's ME - " + CooldownTimer);
        if (CooldownTimer != -1)
            return;

        TeleportingGround[] Tps = GameObject.FindObjectsOfType<TeleportingGround>();
        List<TeleportingGround> Sequence = new List<TeleportingGround>();

        int ThisIndex = 0;        
        for (int x = 0; x < Tps.Length; x++)
        {
            if (Tps[x].GroupNumber == GroupNumber)
            {
                if (Tps[x].gameObject == gameObject)
                {
                    Debug.Log("Oh, I'm the " + Sequence.Count + " teleporter.");
                    ThisIndex = Sequence.Count;
                }
                Sequence.Add(Tps[x]);                
            }
        }

        Debug.Log("There are " + Sequence.Count + " teleporters in the scene.");

        //Figure out which teleporter is the destination...
        int TargetIndex = ThisIndex + 1;
        if (TargetIndex > Sequence.Count-1)
        {
            TargetIndex = 0;
        }

        CooldownTimer = 5;
        Sequence[TargetIndex].CooldownTimer = 5;

        //Teleport me to the target...
        Debug.Log("Teleporting To " + TargetIndex + " @ " + Sequence[TargetIndex].transform.position);
        Vector3 Offset = transform.position - P.transform.position;
        P.Teleport(Sequence[TargetIndex].transform.position + Offset);

        if (TPSound != null)
        {
            AudioSource ASource = gameObject.GetComponent<AudioSource>();
            if (ASource == null)
            {
                ASource = gameObject.AddComponent<AudioSource>();
            }
            ASource.clip = TPSound;
            ASource.Play();
        }

    }
}
