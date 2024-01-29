using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public LandPlacement LP;
    public CameraMovement CM;
    public CameraTracker CT;
    public GameObject Player;
    public float MaxSpeed = 1;
    public float Friction = 2;
    public bool TickTock = false;
    public EditorScene EditInterface;

    public static PlayerControl Instance;

    public Color StopColour = Color.red;
    public UnityEngine.UI.Image PlayImage;
    public Sprite StopIcon;


    public float Gravity = 5;
    public float JumpPower = 20;
    public float JumpDuration = 1;

    public bool FlightMode = false;

    float ActiveJumpDuration = 0;
    float UpSpeed = 0;

    Vector3 Initial = Vector3.zero;

    bool Disabled = false;
    Sprite OldPlayImage;
    Color OldPlayColour;

    // Start is called before the first frame update
    void Start()
    {
        Initial = Player.transform.position;
        Instance = this;
        P = Player.GetComponentInChildren<Player>();
    }

    public enum GameMode { playing, terrain, prop };
    public GameMode CurrentGameMode = GameMode.terrain;

    public void Respawn()
    {
        LP.enabled = false;
        CM.enabled = false;
        CT.enabled = true;
        OldPlayColour = PlayImage.color;
        OldPlayImage = PlayImage.sprite;
        PlayImage.sprite = StopIcon;
        PlayImage.color = StopColour;
        UpSpeed = 0;
        P.Velocity = Vector3.zero;
        BuilderPanel.SetActive(false);
        GameTimer = 120;
        CurrentGameMode = GameMode.playing;
        Player.GetComponent<Player>().Restart();
    }

    public void Toggle()
    {
        Player P = Player.GetComponentInChildren<Player>();
        if (LP.enabled == true)
        {
            LP.enabled = false;
            CM.enabled = false;
            CT.enabled = true;
            TickTockTimer = Time.time;
            OldPlayColour = PlayImage.color;
            OldPlayImage = PlayImage.sprite;
            PlayImage.sprite = StopIcon;
            PlayImage.color = StopColour;
            UpSpeed = 0;
            P.Velocity = Vector3.zero;
            BuilderPanel.SetActive(false);
            GameTimer = 120;
            CurrentGameMode = GameMode.playing;
            P.Restart();                        
            gameObject.GetComponent<MusicManager>().PlaySong(OptionCore.Instance.LevelMusic);
            Appearance[] Apps = GameObject.FindObjectsOfType<Appearance>();
            foreach (Appearance A in Apps)
            {
                A.enabled = true;
            }
            PopupUI PU = GameObject.FindObjectOfType<PopupUI>();
            try
            {
                PU.gameObject.SetActive(false);
            }
            catch
            {
            }

            GoLive[] LiveElements = GameObject.FindObjectsOfType<GoLive>();
            foreach (GoLive GL in LiveElements)
            {
                GL.PlayMode();
            }

            EditorOnly[] Objects = GameObject.FindObjectsOfType<EditorOnly>();
            foreach (EditorOnly G in Objects)
            {
                G.SendMessage("StartPlayMode");
            }
            EditorScene.Instance.Restart();
        }
        else
        {
            LP.enabled = true;
            CM.enabled = true;
            CT.enabled = false;
            Player.transform.position = Initial;
            Animator A = Player.GetComponent<Animator>();
            A.SetBool("Dead", false);
            Disabled = false;
            PlayImage.sprite = OldPlayImage;
            PlayImage.color = OldPlayColour;
            BuilderPanel.SetActive(true);
            CurrentGameMode = GameMode.terrain;
            P.Stop();
            EditInterface.ResetAll();

            GameObject[] Objs = GameObject.FindObjectsOfType<GameObject>();
            foreach (GameObject G in Objs)
            {               
                if (G.layer == 12)
                {
                    Animator Ax = GetComponent<Animator>();
                    if (Ax == null)
                    {
                        if (G.transform.parent != null)
                        {
                            Ax = G.transform.parent.gameObject.GetComponent<Animator>();
                        }
                    }
                    if (Ax != null) Ax.SetBool("Triggered", false);
                }
            }

            gameObject.GetComponent<MusicManager>().PlaySong(0);
            Appearance[] Apps = GameObject.FindObjectsOfType<Appearance>();
            foreach (Appearance Ap in Apps)
            {
                Ap.enabled = false;
            }

            GoLive[] LiveElements = GameObject.FindObjectsOfType<GoLive>();
            foreach (GoLive GL in LiveElements)
            {
                GL.EditMode();
            }

            EditorOnly[] Objects = GameObject.FindObjectsOfType<EditorOnly>();
            foreach (EditorOnly G in Objects)
            {
                G.SendMessage("StartEditMode");
            }
        }
    }

    public UnityEngine.UI.Text TimerText;
    public float GameTimer = 120;
    public LayerMask GroundMask = 0;
    bool WasAirborne = false;
    Player P = null;
    Quaternion LastFacing = Quaternion.identity;

    public Vector3 ForwardVector = Vector3.forward;
    public Vector3 RayOffset = Vector3.zero;

    int JumpCount = 0;
    int JumpsAllowed = 2;

    float TickTockTimer = 0;
    int TickTockSpeed = 4;

    public int PropertyLayer = 0;
    Grounded LastGround = null;

    // Update is called once per frame
    void Update()
    {
        if (LP.enabled) return;
        if (Disabled == true) return;
        float Snap = 0.05f;
        float ActualFriction = Friction;

        GameTimer -= Time.deltaTime;
        if (GameTimer % 60 < 10)
            TimerText.text = ((int)GameTimer / 60) + ":0" + (int)(GameTimer % 60);
        else
            TimerText.text = ((int)GameTimer / 60) + ":" + (int)(GameTimer % 60);

        if ((Time.time - TickTockTimer) > TickTockSpeed)
        {
            TickTock = !TickTock;
            TickTockTimer = Time.time;
        }

        if (GameTimer < 0)
        {
            Player P = Player.GetComponentInChildren<Player>();
            P.Kill();
            Disabled = true;
            GameTimer = 0;
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            Respawn();
        }

        if (P.Lives <= 0) return;
        
        
        float Horiz = Input.GetAxis("Horizontal");
        float Vert = Input.GetAxis("Vertical");        

        //CHEATS
        if (Input.GetKeyDown(KeyCode.F))
        {
            FlightMode = !FlightMode;
        }
        //END CHEATS
        bool GroundHit = false;        
        
        RaycastHit H = new RaycastHit();
        //Check for the ground.
        if (Physics.Raycast(Player.transform.position+(Vector3.up*Snap), -Vector3.up, out H, 100, GroundMask, QueryTriggerInteraction.Ignore))
        {
            float Dist = (H.point - Player.transform.position).magnitude;            
            if (Dist < Snap)
            {
                if (P.Velocity.y <= 0)
                {
                    Player.transform.position = new Vector3(Player.transform.position.x, H.point.y, Player.transform.position.z);
                }
                //If it's damaging, let's take damage.     
                Grounded I = H.collider.gameObject.GetComponent<Grounded>();
                if (I != null)
                {
                    //Debug.Log("Found a ground behaviour!");
                    P.ImpactDirection = Vector3.down * 0.75f;
                    I.GroundHit(P);
                    LastGround = I;
                    GroundHit = true;
                }                

                if (P.Velocity.y < 0)
                    P.Velocity.y = 0;

                if (H.collider.gameObject.name.Contains("Lava"))
                {                    
                    Disabled = true;
                    Player P = Player.GetComponentInChildren<Player>();
                    P.Kill();
                }
                if (H.collider.gameObject.name.Contains("Sand"))
                {
                    //Horiz *= 0.5f;
                    //Vert *= 0.5f;
                    ActualFriction *= 2;
                }
                if (H.collider.gameObject.name.Contains("Water"))
                {
                    //Horiz *= 0.5f;
                    //Vert *= 0.5f;
                    ActualFriction *= 2;
                }
                if (H.collider.gameObject.name.Contains("Ice"))
                {
                    ActualFriction *= 0.2f;
                }
                if (H.collider.gameObject.name.Contains("Finish"))
                {
                    Toggle();
                    SceneEffects.Instance.DropEffect(H.point, H.normal, Vector3.one, "Win");
                }
                if (H.collider.gameObject.name.Contains("OnOff"))
                {
                    if (H.collider.gameObject.GetComponent<MeshRenderer>().enabled == false)
                    {
                        Player P = Player.GetComponentInChildren<Player>();
                        P.Kill();
                        Disabled = true;
                    }
                }

                

                if (ActiveJumpDuration < JumpDuration)
                {
                    ActiveJumpDuration += Time.deltaTime;
                    if (ActiveJumpDuration > JumpDuration)
                        ActiveJumpDuration = JumpDuration;
                }

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    Toggle();
                }

                if ((Input.GetKeyDown(KeyCode.LeftControl)) || (Input.GetKeyDown(KeyCode.Space)))
                {
                    if ((ActiveJumpDuration > 0) && (JumpsAllowed >= 1))
                    {
                        JumpCount = 1;
                        P.Velocity.y = JumpPower;
                        SceneEffects.Instance.DropEffect(H.point, H.normal, Vector3.one,"JumpTakeoff");
                        if (H.collider.gameObject.name.Contains("Spring"))
                        {
                            P.Velocity.y = JumpPower*4;
                        }
                        if (H.collider.gameObject.name.Contains("Slime"))
                        {
                            P.Velocity.y = 0;
                        }
                        ActiveJumpDuration -= Time.deltaTime;
                    }
                }

                if(WasAirborne == true)
                {
                    string Lander = "Grass";
                    Lander = H.collider.gameObject.name;
                    SceneEffects.Instance.DropEffect(H.point, H.normal, Vector3.one, Lander,"Grass");
                    WasAirborne = false;
                }
                

            }
            else
            {
                WasAirborne = true;
                P.Velocity.y -= Gravity * Time.deltaTime;
                if ((Input.GetKeyDown(KeyCode.LeftControl)) || (Input.GetKeyDown(KeyCode.Space)))
                {
                    if (JumpCount < JumpsAllowed)
                    {
                        P.Velocity.y = JumpPower;
                        SceneEffects.Instance.DropEffect(H.point, H.normal, Vector3.one, "JumpTakeoff");
                        JumpCount++;
                    }
                }
            }
        }
        else
        {
            //Debug.Log("I've Fallen Off!");
            if (FlightMode == false)
            {
                P.Velocity.y -= Gravity * Time.deltaTime;
                if (Player.transform.position.y < -0)
                {
                    P.Velocity.y = 0;                    
                    Disabled = true;
                    //Player P = Player.GetComponentInChildren<Player>();
                    P.Kill();
                }
            }
            ActualFriction *= 0.1f;

            if ((Input.GetKeyDown(KeyCode.LeftControl)) || (Input.GetKeyDown(KeyCode.Space)))
            {
                if (JumpCount < JumpsAllowed)
                {
                    P.Velocity.y = JumpPower;
                    SceneEffects.Instance.DropEffect(H.point, H.normal, Vector3.one, "JumpTakeoff");
                    JumpCount++;
                }
            }
        }

        if (LastGround != null)
        {
            if (GroundHit == false)
            {
                LastGround.GroundLeft(P);
                LastGround = null;
            }
        }

        ActualFriction *= (0.5f + P.Velocity.magnitude);
        Vector3 FlatV = P.Velocity;
        FlatV.y = 0;

        if (FlatV.magnitude != 0)
        {
            LastFacing = Quaternion.LookRotation(FlatV, Vector3.up) * Quaternion.Euler(0, 90, 0);
            Player.transform.rotation = Quaternion.Lerp(Player.transform.rotation,LastFacing, 0.25f);
        }
        else
        {
            Player.transform.rotation = Quaternion.Lerp(Player.transform.rotation, LastFacing, 0.25f);
        }        

        float Amnt = (ActualFriction * 0.75f) * Time.deltaTime;

        Vector3 Tweaker = P.Velocity;
        if (Mathf.Abs(Tweaker.x) < Amnt)
            Tweaker.x = 0;
        else
        {
            if (Tweaker.x > 0)
                Tweaker.x = Tweaker.x - Amnt;
            else
                Tweaker.x = Tweaker.x + Amnt;

        }

        if (Mathf.Abs(Tweaker.z) < Amnt)
            Tweaker.z = 0;
        else
        {
            if (Tweaker.z > 0)
                Tweaker.z = Tweaker.z - Amnt;
            else
                Tweaker.z = Tweaker.z + Amnt;
        }

        Tweaker.x += Horiz * MaxSpeed * Time.deltaTime;
        Tweaker.z += Vert * MaxSpeed * Time.deltaTime;
        //Tweaker.y = UpSpeed;
                
        P.Velocity = Tweaker;
        RaycastHit Ht = new RaycastHit();

        //Check to see if we hit something!
        if (Physics.Raycast(Player.transform.position + RayOffset, Player.transform.TransformVector(ForwardVector).normalized, out Ht, ForwardVector.magnitude * 0.5f, GroundMask, QueryTriggerInteraction.Ignore))
        {            
            //Debug.Log("Struck Collider " + Ht.collider.name);

            //Kill off all velocity IN THE DIRECTION OF TRAVEL.
            Vector3 Stp = Player.transform.TransformVector(ForwardVector).normalized;
            if ((Stp.x > 0) && (Tweaker.x > 0)) Tweaker.x = 0;
            if ((Stp.z > 0) && (Tweaker.z > 0)) Tweaker.z = 0;

            if ((Stp.x < 0) && (Tweaker.x < 0)) Tweaker.x = 0;
            if ((Stp.z < 0) && (Tweaker.z < 0)) Tweaker.z = 0;
            P.Velocity = Tweaker;       

        }
        else
        {
            //Debug.Log("Nothing!");
        }
        Player.transform.position += Tweaker * Time.deltaTime;
        //Player.transform.position += new Vector3(Horiz * MaxSpeed, UpSpeed, Vert * MaxSpeed) * Time.deltaTime;


    }

    public void Teleport(Vector3 Pos)
    {
        Player.transform.position = Pos;
        P.Velocity = Vector3.zero;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(Player.transform.position + RayOffset, Player.transform.position + RayOffset + (Player.transform.TransformVector(ForwardVector) * ForwardVector.magnitude * 0.5f));
    }


    public GameObject BuilderPanel;

}
