using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
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

    private PlayerState EnemyHealthScript;

    private PlayerController EnemyControllerScript;

    private GameObject EnemyHead;

    // Button Presses
    public KeyCode MoveLeftButton;
    public KeyCode MoveRightButton;

    public KeyCode PunchLeftButton;
    public KeyCode PunchRightButton;

    // for rotation only
    public bool RedTeam;

    // Use this for initialization
    void Start()
    {
        // ****TODO: Rewrite
        mCenter = mTarget.transform.position;
        if (RedTeam)
        {
            EnemyHealthScript = (PlayerState)GameObject.FindGameObjectWithTag("BlueDude").GetComponent(typeof(PlayerState));
            EnemyControllerScript = (PlayerController)GameObject.FindGameObjectWithTag("BlueDude").GetComponent(typeof(PlayerController));
            EnemyHead = GameObject.FindGameObjectWithTag("BlueHead");
        }
        else
        {
            EnemyHealthScript = (PlayerState)GameObject.FindGameObjectWithTag("RedDude").GetComponent(typeof(PlayerState));
            EnemyControllerScript = (PlayerController)GameObject.FindGameObjectWithTag("RedDude").GetComponent(typeof(PlayerController));
            EnemyHead = GameObject.FindGameObjectWithTag("RedHead");
        }
        // Set spawn position
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<PlayerState>().getHealth() > 0.0f)
        {
            getInput();
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
    }

    private void Punch(GameObject arm, GameObject fist, float damageTotal)
    {
        arm.transform.position += arm.transform.forward * (damageTotal / 100.0f);

        if (fist.GetComponent<Collider>().bounds.Intersects(EnemyHead.GetComponent<Collider>().bounds))
        {
            EnemyHealthScript.damage(damageTotal);
        }
    }

    // fist = which hand punched
    // ExtnededOut = Where did the extended Punch end after the punch
    // BackToTheSide = the location where the arm will be back on the robo's side
    // SpeedOfMove = Where the arm should be between the two points (the t for the lerp function)
    private void ResetPunchSlow(GameObject fist, Vector3 ExtnededOut, Vector3 BackToTheSide, float SpeedOfMove)
    {
        fist.transform.position = Vector3.Lerp(ExtnededOut, BackToTheSide, SpeedOfMove);
    }

    // True for Left, False for Right
    private void Rotate(bool direction)
    {
        if (RedTeam)
        {
            if (direction && (transform.rotation.eulerAngles.y < 60.0f || transform.rotation.eulerAngles.y > 290.0f))
                transform.RotateAround(mCenter, new Vector3(0, 0.5f, 0), 180 * Time.deltaTime);
            else if (!direction && (transform.rotation.eulerAngles.y > 300.0f || transform.rotation.eulerAngles.y < 70.0f))
                transform.RotateAround(mCenter, -new Vector3(0, 0.5f, 0), 180 * Time.deltaTime);
        }
        else
        {
            if (direction && (transform.rotation.eulerAngles.y < 240.0f && transform.rotation.eulerAngles.y > 110.0f))
                transform.RotateAround(mCenter, new Vector3(0, 0.5f, 0), 180 * Time.deltaTime);
            else if (!direction && (transform.rotation.eulerAngles.y > 120.0f && transform.rotation.eulerAngles.y < 250.0f))
                transform.RotateAround(mCenter, -new Vector3(0, 0.5f, 0), 180 * Time.deltaTime);
        }
    }
}
