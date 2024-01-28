using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wandering : MonoBehaviour
{
    public float MovementSpeed = 2;
    Vector3 OriginPosition;
    Quaternion OriginRotation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void NewPosition()
    {
        OriginPosition = transform.position;
        OriginRotation = transform.rotation;
    }

    void OnEnable()
    {
        OriginPosition = transform.position;
        OriginRotation = transform.rotation;
        Timing = -1;
        SelectNewDestination();
    }

    void OnDisable()
    {
        transform.position = OriginPosition;
        transform.rotation = OriginRotation;
    }

    public LayerMask GroundLayers;

    public void SelectNewDestination()
    {
        Debug.Log(gameObject.name + " is selecting new destination...");
        //Fire off raycasters to determine if there are any nearby floor-spaces...
        List<Vector3> Candidates = new List<Vector3>();
        for (float x = transform.position.x - 1; x < transform.position.x + 2; x++)
        {
            for (float y = transform.position.z - 1; y < transform.position.z + 2; y++)
            {
                if ((x == transform.position.x) || (y == transform.position.z))
                {
                    continue;
                }
                RaycastHit Ht = new RaycastHit();
                if (Physics.Raycast(new Vector3(x, 20, y), Vector3.down,out Ht, 100, GroundLayers))
                {
                    Candidates.Add(Ht.point);
                }
            }
        }

        int Choice = -1;
        if (Direction == MoveDirection.random)
            Choice = Random.Range(0, Candidates.Count + 1);
        if (Direction == MoveDirection.follow)
        {
            float Proximity = -1;
            for(int x=0;x<Candidates.Count;x++)
            {
                Vector3 Dist = Candidates[x] - PlayerControl.Instance.Player.transform.position;
                if ((Proximity == -1) || (Dist.sqrMagnitude < Proximity))
                {
                    Proximity = Dist.sqrMagnitude;
                    Choice = x;
                }
            }
        }
        if (Choice == Candidates.Count)
            Choice -= 1;

        StartPos = transform.position;
        EndPos = Candidates[Choice];
        StartRot = transform.rotation;
        EndRot = Quaternion.LookRotation(StartPos - EndPos, Vector3.up);
        Timing = Time.time;
    }

    float Timing = -1;
    Vector3 StartPos;
    Quaternion StartRot;

    Vector3 EndPos;
    Quaternion EndRot;

    public enum MoveMethod { stepped, smooth };
    public MoveMethod Movement = MoveMethod.smooth;

    public enum MoveDirection { random,path,follow,followclose};
    public MoveDirection Direction = MoveDirection.random;
    public float CloseRadius = 4;

    // Update is called once per frame
    void Update()
    {
        if (Timing != -1)
        {
            float Perc = (Time.time - Timing) / MovementSpeed;
            float TurnPerc = Perc * 4;
            if (TurnPerc > 1) TurnPerc = 1;

            if (Movement == MoveMethod.stepped)
            {
                Perc = Mathf.SmoothStep(0, 1, Perc);
            }
            
            transform.position = Vector3.Lerp(StartPos, EndPos, Perc);
            transform.rotation = Quaternion.Slerp(StartRot, EndRot, TurnPerc);

            if (Perc > 1)
            {
                Timing = -1;
            }
        }
        else
        {
            SelectNewDestination();
        }
    }
}
