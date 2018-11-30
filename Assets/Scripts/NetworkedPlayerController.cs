using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkedPlayerController : NetworkBehaviour
{
    /*
     * Base Input Controller for Player
     * Network Enabled
     */

    // Control Definitions
    const KeyCode LEFT_MOVE_KEY = KeyCode.A;
    const KeyCode RIGHT_MOVE_KEY = KeyCode.D;
    const KeyCode LEFT_PUNCH_KEY = KeyCode.Q;
    const KeyCode RIGHT_PUNCH_KEY = KeyCode.E;

    public Transform mTarget;

    private Vector3 mCenter;
    private float mRadius = 4.0f;

    public GameObject RightArm;
    public GameObject LeftArm;

    public GameObject RightHand;
    public GameObject LeftHand;

    private bool LeftPunch = true;
    private bool RightPunch = true;

    private float LeftArmDistance = 100.0f;
    private float RightArmDistance = 100.0f;

    private float LeftParam = 0f;
    private float RightParam = 0f;

    public GameObject LeftResetLocation;
    public GameObject RightResetLocation;

    private Vector3 LeftExtendedLocation;
    private Vector3 RightExtendedLocation;

    //private BluePlayerMovement EnemyScript;

    private GameObject EnemyHead;

    public GameObject MyHead;

    public Material DeadMaterial;

    private float health = 300.0f;

    private bool RunOnce = true;

    // Button Presses
    public KeyCode MoveLeftButton;
    public KeyCode MoveRightButton;

    public KeyCode PunchLeftButton;
    public KeyCode PunchRightButton;

    // for rotation and player spawn only
    public bool RedTeam;

    // Use this for initialization
    void Start()
    {
        // ****TODO: Rewrite
        mCenter = GameObject.FindGameObjectWithTag("Center").transform.position;
        //mCenter = mTarget.transform.position;
        //EnemyScript = (BluePlayerMovement)GameObject.FindGameObjectWithTag("BlueDude").GetComponent(typeof(BluePlayerMovement));
        //EnemyHead = GameObject.FindGameObjectWithTag("BlueHead");

        // Set spawn position
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        //this.GetComponent<Material>().color = Color.green;
    }

    public virtual void OnServerAddPlayer(NetworkConnection connection, short playerControllerID)
    {
        // Check if client 1
        // Assign color
        // Assign Start Position
    }

    // Update is called once per frame
    void Update()
    {
        if (health > 0.0f)
        {
            if (!isLocalPlayer)
            {
                return;
            }
            if (isLocalPlayer)
            {
                getInput();
            }
        }
        else if (RunOnce)
        {
            MyHead.GetComponent<Renderer>().material = DeadMaterial;
            RunOnce = false;
        }
    }

    public void Damage(float damageDone)
    {
        //Debug.Log("Blue Got Punched" + damageDone);
        if (health > 0.0f)
        {
            health -= damageDone;
            MyHead.transform.position += new Vector3(0.0f, damageDone / 600.0f, 0.0f);
        }
    }

    /* PRIVATE FUNCTIONS */
    private void getInput()
    {
        float speed = 1.5f;

        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            if (Input.GetKey(MoveLeftButton))
            {
                Rotate(true);
            }
            else if (Input.GetKey(MoveRightButton))
            {
                Rotate(false);
            }

            if (Input.GetKey(PunchLeftButton)) // Left punch
            {
                if (LeftPunch)
                {
                    Punch(LeftArm, LeftHand, LeftArmDistance);
                    LeftPunch = false;
                    LeftArmDistance = 0.0f;
                    LeftParam = 0.0f;
                }
            }
            else
            {
                if (!LeftPunch)
                {
                    LeftPunch = true;
                    LeftExtendedLocation = LeftArm.transform.position;
                    //ResetPunchFull(LeftArmPunch);
                }
                if (LeftArmDistance < 100.0f)
                {
                    LeftParam += Time.deltaTime * speed;
                    LeftArmDistance = Mathf.Lerp(0.0f, 100.0f, LeftParam);
                    ResetPunchSlow(LeftArm, LeftExtendedLocation, LeftResetLocation.transform.position, LeftParam);
                }
            }

            if (Input.GetKey(PunchRightButton)) // Right punch
            {
                if (RightPunch)
                {
                    Punch(RightArm, RightHand, RightArmDistance);
                    RightPunch = false;
                    RightArmDistance = 0.0f;
                    RightParam = 0.0f;
                }
            }
            else
            {
                if (!RightPunch)
                {
                    RightPunch = true;
                    RightExtendedLocation = RightArm.transform.position;
                    //ResetPunchFull(RightArmPunch);
                }
                if (RightArmDistance < 100.0f)
                {
                    RightParam += Time.deltaTime * speed;
                    RightArmDistance = Mathf.Lerp(0.0f, 100.0f, RightParam);
                    ResetPunchSlow(RightArm, RightExtendedLocation, RightResetLocation.transform.position, RightParam);
                }
            }
        }
        else
        {
            // Check for left/right tap
            if (Input.touchCount > 0)
            {
                Touch currTouch = Input.GetTouch(0);

                if (currTouch.position.x < Screen.width / 2)        // Punch left
                {
                }
                else if (currTouch.position.x > Screen.width / 2)   // Punch right
                {

                }
            }
        }
        // Check if tilted left/right
        if (Input.acceleration.x > 0.1f)        // Move right
        {
            Rotate(false);
        }
        else if (Input.acceleration.x < -0.1f)  // Move left
        {
            Rotate(true);
        }
    }

    private void Punch(GameObject arm, GameObject fist, float damageTotal)
    {
        arm.transform.position += arm.transform.forward * (damageTotal / 100.0f);

        //if (fist.GetComponent<Collider>().bounds.Intersects(GameObject.FindGameObjectWithTag("BlueHead").GetComponent<Collider>().bounds))
        //{
        //    EnemyScript.Damage(damageTotal);
        //}
    }

    // fist = which hand punched
    // ExtnededOut = Where did the extended Punch end after the punch
    // BackToTheSide = the location where the arm will be back on the robo's side
    // SpeedOfMove = Where the arm should be between the two points (the t for the lerp function)
    private void ResetPunchSlow(GameObject fist, Vector3 ExtnededOut, Vector3 BackToTheSide, float SpeedOfMove)
    {
        fist.transform.position = Vector3.Lerp(ExtnededOut, BackToTheSide, SpeedOfMove);
    }

    private Vector2 PointOnCircle(float angle)
    {
        float x = (mRadius * Mathf.Cos(angle * Mathf.PI / 180.0f) + mCenter.x);
        float y = (mRadius * Mathf.Cos(angle * Mathf.PI / 180.0f) + mCenter.y);

        return new Vector2(x, y);
    }

    // True for Left, False for Right
    private void Rotate(bool direction)
    {
        if (direction)
            transform.RotateAround(mCenter, new Vector3(0, 0.5f, 0), 180 * Time.deltaTime);
        else
            transform.RotateAround(mCenter, -new Vector3(0, 0.5f, 0), 180 * Time.deltaTime);
    }
}
