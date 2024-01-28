using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float Speed = 10;
    public float ScrollSpeed = 5;
    Vector3 SavedPosition = Vector3.zero;
    bool First = true;

    // Start is called before the first frame update
    void Start()
    {
        SavedPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float LR = Input.GetAxis("Horizontal");
        float UD = Input.GetAxis("Vertical");        

        transform.position += new Vector3(LR, 0, UD) * Speed * Time.deltaTime;
        if (PlayerControl.Instance.CurrentGameMode == PlayerControl.GameMode.terrain)
            transform.Translate(new Vector3(0 ,0, Input.mouseScrollDelta.y * ScrollSpeed));

        SavedPosition = transform.position;
    }

    void OnEnable()
    {
        if (First == true)
            First = false;
        else
            transform.position = SavedPosition;
    }

    
}
