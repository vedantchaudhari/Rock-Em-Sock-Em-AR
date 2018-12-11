using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    // Public Var

    // Arena
    public GameObject mTarget;

    // The arms that move to punch
    public GameObject RightArm;
    public GameObject LeftArm;

    // The actual hands that will deal damage
    public GameObject RightHand;
    public GameObject LeftHand;

    // Used for the punching
    public GameObject EnemyHead;

    // for getting other player info
    public bool RedTeam;

    // Where the arm will reset to
    public GameObject LeftResetLocation;
    public GameObject RightResetLocation;

    // Distance arm has moved back and damage they will deal on impact
    public float LeftArmDistance = 100.0f;
    public float RightArmDistance = 100.0f;

    // Private Var

    // Center of the arena
    private Vector3 mCenter;

    // Check if button was pressed to be able to reset stats
    private bool LeftPunch = false;
    private bool RightPunch = false;

    // The time parameter for lerping the arms back and reseting the arm distances
    private float LeftParam = 0f;
    private float RightParam = 0f;

    // The location that the arm has extended to (Used for lerping)
    private Vector3 LeftExtendedLocation;
    private Vector3 RightExtendedLocation;

    // Access to the opponents health
    private PlayerState EnemyHealthScript;

    // movement
    private bool movingLeft;
    private bool movingRight;
    private float moveTotal;
    private float RandoMoveAmount;

    // punching
    private bool AIPunchLeft;
    private bool AIPunchRight;
    private float punchTotal;
    private float RandoPunchAmount;

    // Use this for initialization
    void Start()
    {
        // get center of stage
        mTarget = GameObject.FindGameObjectWithTag("Center");
        mCenter = mTarget.transform.position;

        // used to grab the correct information from the opponent
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

        // Used to start the movement and punching right away
        RandoMoveAmount = Random.Range(0.0f, 0.0f);
        RandoPunchAmount = Random.Range(2.0f, 4.0f);
        // Set spawn position
    }

    // Update is called once per frame
    void Update()
    {
        // Moving speed
        float speed = 1.5f;

        // check if alive
        if (GetComponent<PlayerState>().getHealth() > 0.0f)
        {
            // check if ready to get new movement
            if (moveTotal >= RandoMoveAmount)
            {
                RandoMoveAmount = Random.Range(0.0f, 1.0f);
                moveTotal = 0.0f;
                int choice = Random.Range(1, 3);
                // chooses which way to move
                switch (choice)
                {
                    case 1: // move left
                        if (transform.rotation.eulerAngles.y < 240.0f && transform.rotation.eulerAngles.y > 110.0f)
                        {
                            movingLeft = true;
                            movingRight = false;
                        }
                        else
                        {
                            movingLeft = false;
                            movingRight = true;
                        }
                        break;
                    case 2: // move right
                        if (transform.rotation.eulerAngles.y > 120.0f && transform.rotation.eulerAngles.y < 250.0f)
                        {
                            movingLeft = false;
                            movingRight = true;
                        }
                        else
                        {
                            movingLeft = true;
                            movingRight = false;
                        }
                        break;
                        //case 3: // don't move
                        //    movingLeft = false;
                        //    movingRight = false;
                        //    break;
                }
            }
            // increment time until next movement choice
            moveTotal += Time.deltaTime;
            // move
            if (movingLeft)
            {
                if (transform.rotation.eulerAngles.y < 240.0f && transform.rotation.eulerAngles.y > 110.0f)
                {
                    RotateLeft();
                }
            }
            else if (movingRight)
            {
                if (transform.rotation.eulerAngles.y > 120.0f && transform.rotation.eulerAngles.y < 250.0f)
                {
                    RotateRight();
                }
            }



            // Punching
            // Reset punch if punch happened
            if (!LeftPunch)
            {
                LeftPunch = true;
                LeftExtendedLocation = LeftArm.transform.position;
            }
            // If arm not back move arm back
            else if (LeftArmDistance < 100.0f)
            {
                LeftParam += Time.deltaTime * speed;
                LeftArmDistance = Mathf.Lerp(0.0f, 100.0f, LeftParam);
                ResetPunchSlow(LeftArm, LeftExtendedLocation, LeftResetLocation.transform.position, LeftParam);
            }
            // Reset punch if punch happened
            if (!RightPunch)
            {
                RightPunch = true;
                RightExtendedLocation = RightArm.transform.position;
            }
            // If arm not back move arm back
            else if (RightArmDistance < 100.0f)
            {
                RightParam += Time.deltaTime * speed;
                RightArmDistance = Mathf.Lerp(0.0f, 100.0f, RightParam);
                ResetPunchSlow(RightArm, RightExtendedLocation, RightResetLocation.transform.position, RightParam);
            }
            // If punch timer is reached then punch
            if (punchTotal >= RandoPunchAmount)
            {
                RandoPunchAmount = Random.Range(2.0f, 4.0f);
                punchTotal = 0.0f;
                int choice = Random.Range(1, 3);
                // determine which arm to punch with
                switch (choice)
                {
                    case 1: // Punch Left
                        Punch(LeftArm, LeftHand, LeftArmDistance);
                        LeftArmDistance = 0.0f;
                        LeftPunch = false;
                        LeftParam = 0.0f;
                        break;
                    case 2: // move right
                        Punch(RightArm, RightHand, RightArmDistance);
                        RightArmDistance = 0.0f;
                        RightPunch = false;
                        RightParam = 0.0f;
                        break;
                }
            }
            // increment timer for next punch
            punchTotal += Time.deltaTime;
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
    
    // Rotate to the left
    private void RotateLeft()
    {
        transform.RotateAround(mCenter, new Vector3(0, 0.5f, 0), 270 * Time.deltaTime);
    }

    // Rotate to the right
    private void RotateRight()
    {
        transform.RotateAround(mCenter, -new Vector3(0, 0.5f, 0), 270 * Time.deltaTime);
    }


    // True for Left, False for Right
    // Unused Code
    private void Rotate(bool direction)
    {
        if (RedTeam)
        {
            if (direction && (transform.rotation.eulerAngles.y < 60.0f || transform.rotation.eulerAngles.y > 290.0f))
                transform.RotateAround(mCenter, new Vector3(0, 0.5f, 0), 270 * Time.deltaTime);
            else if (!direction && (transform.rotation.eulerAngles.y > 300.0f || transform.rotation.eulerAngles.y < 70.0f))
                transform.RotateAround(mCenter, -new Vector3(0, 0.5f, 0), 270 * Time.deltaTime);
        }
        else
        {
            if (direction && (transform.rotation.eulerAngles.y < 240.0f && transform.rotation.eulerAngles.y > 110.0f))
                transform.RotateAround(mCenter, new Vector3(0, 0.5f, 0), 270 * Time.deltaTime);
            else if (!direction && (transform.rotation.eulerAngles.y > 120.0f && transform.rotation.eulerAngles.y < 250.0f))
                transform.RotateAround(mCenter, -new Vector3(0, 0.5f, 0), 270 * Time.deltaTime);
        }
    }
}
