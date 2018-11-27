using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public Transform mTarget;

    private Vector3 mCenter;
    private float mRadius = 4.0f;

    public GameObject RightArmPunch;
    public GameObject LeftArmPunch;

    public GameObject RightArmHand;
    public GameObject LeftArmHand;

    private bool LeftPunch = true;
    private bool RightPunch = true;

    private float LeftArmDistance = 100.0f;
    private float RightArmDistance = 100.0f;

    private float LeftParam = 0f;
    private float RightParam = 0f;

    public GameObject LeftResetLocation;
    public GameObject RightResetLocation;

    public GameObject LeftExtendPoint;
    public GameObject RightExtendPoint;

    private Vector3 LeftExtendedLocation;
    private Vector3 RightExtendedLocation;

    private PlayerState EnemyScript;

    private PlayerState HealthScript;

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

    // Use this for initialization
    void Start()
    {
        mCenter = mTarget.transform.position;
        EnemyScript = (PlayerState)GameObject.FindGameObjectWithTag("BlueDude").GetComponent(typeof(PlayerState));
        EnemyHead = GameObject.FindGameObjectWithTag("BlueHead");
    }

    // Update is called once per frame
    void Update()
    {
        if (health > 0.0f)
        {
            getInput();
        }
        else if (RunOnce)
        {
            MyHead.GetComponent<Renderer>().material = DeadMaterial;
            RunOnce = false;
        }
    }

    public void Damage(float damageDone)
    {
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
                    Punch(LeftArmPunch, LeftArmHand, LeftExtendPoint, LeftArmDistance);
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
                    LeftExtendedLocation = LeftArmPunch.transform.position;
                    LeftExtendPoint.transform.position -= LeftExtendPoint.transform.forward * 1.0f;
                    //ResetPunchFull(LeftArmPunch);
                }
                if (LeftArmDistance < 100.0f)
                {
                    LeftParam += Time.deltaTime * speed;
                    LeftArmDistance = Mathf.Lerp(0.0f, 100.0f, LeftParam);
                    ResetPunchSlow(LeftArmPunch, LeftExtendedLocation, LeftResetLocation.transform.position, LeftParam);
                }
            }

            if (Input.GetKey(PunchRightButton)) // Right punch
            {
                if (RightPunch)
                {
                    Punch(RightArmPunch, RightArmHand, RightExtendPoint, RightArmDistance);
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
                    RightExtendedLocation = RightArmPunch.transform.position;
                    RightExtendPoint.transform.position -= RightExtendPoint.transform.forward * 1.0f;
                    //ResetPunchFull(RightArmPunch);
                }
                if (RightArmDistance < 100.0f)
                {
                    RightParam += Time.deltaTime * speed;
                    RightArmDistance = Mathf.Lerp(0.0f, 100.0f, RightParam);
                    ResetPunchSlow(RightArmPunch, RightExtendedLocation, RightResetLocation.transform.position, RightParam);
                }
            }
        }
        else
        {   // SETUP FOR MOBILE CONTROLS

        }
    }

    private void Punch(GameObject arm, GameObject fist, GameObject ExtendPoint, float damageTotal)
    {
        ExtendPoint.transform.position += ExtendPoint.transform.forward * 1.0f;
        arm.transform.position = ExtendPoint.transform.position;

        if (fist.GetComponent<Collider>().bounds.Intersects(GameObject.FindGameObjectWithTag("BlueHead").GetComponent<Collider>().bounds))
        {
            EnemyScript.damage(damageTotal);
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

    //private void ResetPunchFull(GameObject fist)
    //{
    //    fist.transform.position -= fist.transform.forward * 1.0f;
    //}


    private Vector2 PointOnCircle(float angle)
    {
        float x = (mRadius * Mathf.Cos(angle * Mathf.PI / 180.0f) + mCenter.x);
        float y = (mRadius * Mathf.Cos(angle * Mathf.PI / 180.0f) + mCenter.y);

        return new Vector2(x, y);
    }

    // True for Left, False for Right
    private void Rotate(bool direction)
    {
        if (direction && (transform.rotation.eulerAngles.y < 60.0f || transform.rotation.eulerAngles.y > 290.0f))
            transform.RotateAround(mCenter, new Vector3(0, 0.5f, 0), 180 * Time.deltaTime);
        else if (!direction && (transform.rotation.eulerAngles.y > 300.0f || transform.rotation.eulerAngles.y < 70.0f))
            transform.RotateAround(mCenter, -new Vector3(0, 0.5f, 0), 180 * Time.deltaTime);
    }
}
