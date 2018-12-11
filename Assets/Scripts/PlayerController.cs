using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Public

    // Arena
    public GameObject mTarget;

    // Used for the punching
    public GameObject EnemyHead;

    // Button Presses
    public KeyCode MoveLeftButton;
    public KeyCode MoveRightButton;
    public KeyCode PunchLeftButton;
    public KeyCode PunchRightButton;

    // for getting other player info
    public bool RedTeam;

    // The arms that move to punch
    public GameObject RightArm;
    public GameObject LeftArm;

    // The actual hands that will deal damage
    public GameObject RightHand;
    public GameObject LeftHand;

    // Check if button was pressed to be able to reset stats
    private bool LeftPunch = false;
    private bool RightPunch = false;

    // Distance arm has moved back and damage they will deal on impact
    public float LeftArmDistance = 100.0f;
    public float RightArmDistance = 100.0f;

    // The time parameter for lerping the arms back and reseting the arm distances
    private float LeftParam = 0f;
    private float RightParam = 0f;

    // Where the arm will reset to
    public GameObject LeftResetLocation;
    public GameObject RightResetLocation;

    // The location that the arm has extended to (Used for lerping)
    private Vector3 LeftExtendedLocation;
    private Vector3 RightExtendedLocation;

    // Access to the opponents health
    private PlayerState EnemyHealthScript;

    // Center of stage
    private Vector3 mCenter;

    // Use this for initialization
    void Start()
    {
        mTarget = GameObject.FindGameObjectWithTag("Center");

        mCenter = mTarget.transform.position;

        if (RedTeam)
        {
            EnemyHealthScript = (PlayerState)GameObject.FindGameObjectWithTag("BlueDude").GetComponent(typeof(PlayerState));
            //EnemyControllerScript = (PlayerController)GameObject.FindGameObjectWithTag("BlueDude").GetComponent(typeof(PlayerController));
            EnemyHead = GameObject.FindGameObjectWithTag("BlueHead");
        }
        else
        {
            EnemyHealthScript = (PlayerState)GameObject.FindGameObjectWithTag("RedDude").GetComponent(typeof(PlayerState));
            //EnemyControllerScript = (PlayerController)GameObject.FindGameObjectWithTag("RedDude").GetComponent(typeof(PlayerController));
            EnemyHead = GameObject.FindGameObjectWithTag("RedHead");
        }
        // Set spawn position
    }

    // Update is called once per frame
    void Update()
    {
        // check to see if alive
        if (GetComponent<PlayerState>().getHealth() > 0.0f)
        {
            // reset punch distance
            if(!RightPunch)
            {
                RightArmDistance = 0.0f;
            }
            if(!LeftPunch)
            {
                LeftArmDistance = 0.0f;
            }
            getInput();
        }
    }

    /* PRIVATE FUNCTIONS */
    private void getInput()
    {
        float speed = 1.5f;

        // check which device is being used
        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            // check for rotation
            if (Input.GetKey(MoveLeftButton))
            {
                Rotate(true);
            }
            else if (Input.GetKey(MoveRightButton))
            {
                Rotate(false);
            }

            //check for punches
            if (Input.GetKey(PunchLeftButton)) // Left punch
            {
                // punch happened
                if (LeftPunch)
                {
                    Punch(LeftArm, LeftHand, LeftArmDistance);
                    LeftPunch = false;
                    //LeftArmDistance = 0.0f;
                    LeftParam = 0.0f;
                }
            }
            else
            {
                // reset punches
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

            // check punches
            if (Input.GetKey(PunchRightButton)) // Right punch
            {
                // punch happened
                if (RightPunch)
                {
                    Punch(RightArm, RightHand, RightArmDistance);
                    RightPunch = false;
                    //RightArmDistance = 0.0f;
                    RightParam = 0.0f;
                }
            }
            else
            {
                // reset punches
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
        // connected to phone
        else
        {
            // Check if tilted left/right
            if (Input.acceleration.x > 0.1f)        // Move right
            {
                Rotate(false);
            }
            else if (Input.acceleration.x < -0.1f)  // Move left
            {
                Rotate(true);
            }
            // Check for left/right tap
            if (Input.touchCount > 0)
            {
                Touch currTouch = Input.GetTouch(0);

                if (currTouch.position.x < Screen.width / 2)        // Punch left
                {
                    // punch
                    if (LeftPunch)
                    {
                        Punch(LeftArm, LeftHand, LeftArmDistance);
                        LeftPunch = false;
                        //LeftArmDistance = 0.0f;
                        LeftParam = 0.0f;
                    }
                }
                else
                {
                    // reset punch
                    if (!LeftPunch)
                    {
                        LeftPunch = true;
                        LeftExtendedLocation = LeftArm.transform.position;
                    }
                    if (LeftArmDistance < 100.0f)
                    {
                        LeftParam += Time.deltaTime * speed;
                        LeftArmDistance = Mathf.Lerp(0.0f, 100.0f, LeftParam);
                        ResetPunchSlow(LeftArm, LeftExtendedLocation, LeftResetLocation.transform.position, LeftParam);
                    }
                }
                if (currTouch.position.x > Screen.width / 2)   // Punch right
                {
                    // punch
                    if (RightPunch)
                    {
                        Punch(RightArm, RightHand, RightArmDistance);
                        RightPunch = false;
                        //RightArmDistance = 0.0f;
                        RightParam = 0.0f;
                    }
                }
                else
                {
                    // reset punch
                    if (!RightPunch)
                    {
                        RightPunch = true;
                        RightExtendedLocation = RightArm.transform.position;
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
                // move punches back if not punching
                if (!LeftPunch)
                {
                    LeftPunch = true;
                    LeftExtendedLocation = LeftArm.transform.position;
                }
                if (LeftArmDistance < 100.0f)
                {
                    LeftParam += Time.deltaTime * speed;
                    LeftArmDistance = Mathf.Lerp(0.0f, 100.0f, LeftParam);
                    ResetPunchSlow(LeftArm, LeftExtendedLocation, LeftResetLocation.transform.position, LeftParam);
                }
                if (!RightPunch)
                {
                    RightPunch = true;
                    RightExtendedLocation = RightArm.transform.position;
                }
                if (RightArmDistance < 100.0f)
                {
                    RightParam += Time.deltaTime * speed;
                    RightArmDistance = Mathf.Lerp(0.0f, 100.0f, RightParam);
                    ResetPunchSlow(RightArm, RightExtendedLocation, RightResetLocation.transform.position, RightParam);
                }
            }
        }
    }

    // Thrust the arm forward
    // arm = the arm being thrusted
    // fist = the hand of the arm
    // damageTotal = the arm distance of the respective arm thrusted
    private void Punch(GameObject arm, GameObject fist, float damageTotal)
    {
        arm.transform.position += arm.transform.forward * (damageTotal / 100.0f) * this.transform.localScale.x;
    }

    // damageDone = the arm distance of the respective arm thrusted
    // Used to deal damage to the enemy player
    public void hit(float damageDone)
    {
        EnemyHealthScript.damage(damageDone, this.transform.localScale.x);
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
                transform.RotateAround(mCenter, new Vector3(0, 1.0f, 0), 250 * Time.deltaTime);
            else if (!direction && (transform.rotation.eulerAngles.y > 300.0f || transform.rotation.eulerAngles.y < 70.0f))
                transform.RotateAround(mCenter, -new Vector3(0, 1.0f, 0), 250 * Time.deltaTime);
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
