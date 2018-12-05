using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class NetworkedPlayerControllerPhoton : MonoBehaviourPunCallbacks, IPunObservable
{
    #region Public Variables
    public static GameObject localPlayerInstance;
    public Vector3 mCenter;

    public GameObject Head;
    public GameObject RightArm;
    public GameObject LeftArm;
    public GameObject RightHand;
    public GameObject LeftHand;
    public GameObject LeftResetLocation;
    public GameObject RightResetLocation;

    public KeyCode MoveLeftButton;
    public KeyCode MoveRightButton;
    public KeyCode PunchLeftButton;
    public KeyCode PunchRightButton;
    public Text healthText;
    public bool isRedTeam;

    #endregion

    #region Private Variables

    private bool LeftPunch = true;
    private bool RightPunch = true;
    private float LeftArmDistance = 100.0f;
    private float RightArmDistance = 100.0f;
    private float LeftParam = 0f;
    private float RightParam = 0f;
    private Vector3 LeftExtendedLocation;
    private Vector3 RightExtendedLocation;
    private PlayerState EnemyHealthScript;
    private NetworkedPlayerControllerPhoton EnemyControllerScript;
    private GameObject EnemyHead;
    private bool didLoadEnemyData = false;
    private float health = 300.0f;

    /* Networked Variables */
    private bool isDamaged = false;
    private float dmgAmountNet;

    #endregion

    #region MonoBehaviour Callbacks

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        PhotonNetwork.SendRate = 30;

        if (photonView.IsMine)
        {
            NetworkedPlayerControllerPhoton.localPlayerInstance = this.gameObject;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        mCenter = GameObject.FindGameObjectWithTag("Center").transform.position;
        healthText = GameObject.FindGameObjectWithTag("HealthText").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if enemy instance is null
        if (!didLoadEnemyData && PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            if (isRedTeam)
            {
                EnemyControllerScript = (NetworkedPlayerControllerPhoton)GameObject.FindGameObjectWithTag("BlueDude").GetComponent(typeof(NetworkedPlayerControllerPhoton));
                EnemyHead = GameObject.FindGameObjectWithTag("BlueHead");
                didLoadEnemyData = true;
            }
            else
            {
                EnemyControllerScript = (NetworkedPlayerControllerPhoton)GameObject.FindGameObjectWithTag("RedDude").GetComponent(typeof(NetworkedPlayerControllerPhoton));
                EnemyHead = GameObject.FindGameObjectWithTag("RedHead");
                didLoadEnemyData = true;
            }
        }

        if (photonView.IsMine && PhotonNetwork.IsConnected)
        {
            GetInput();
        }
        if (health <= 0.0f)
        {
            Debug.Log("ACTIONS SPEAK LOUDER THAN WORDS, LET ME TRY THIS SHIT\nDEAD");
            GameManagerPhoton.Instance.LeaveRoom();
        }

        healthText.text = "Health: " + health;
    }

    #endregion

    #region Private Functions

    private void GetInput()
    {
        float speed = 1.5f;

        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            if (Input.GetKey(MoveLeftButton))
            {
                Rotate(true);
            }
            if (Input.GetKey(MoveRightButton))
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
            
        }
    }

    private void Punch(GameObject arm, GameObject fist, float damageTotal)
    {
        arm.transform.position += arm.transform.forward * (damageTotal / 100.0f);

        if (fist.GetComponent<Collider>().bounds.Intersects(EnemyHead.GetComponent<Collider>().bounds))
        {
            Debug.LogAssertionFormat("Local player {0} damaged enemy for {1} health", localPlayerInstance.name, damageTotal);

            dmgAmountNet = damageTotal;
            isDamaged = true;   // Flag to send over networked
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

    private void Rotate(bool direction) // True for left, False for right
    {
        if (isRedTeam)
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

    private void Damage(float amount)
    {
        health -= amount;
        Debug.Log("Current Health is: " + health);
        //Head.GetPhotonView().transform.position += new Vector3(0.00ff, amount / 600.0f, 0.);
        //head.transform.position  += new Vector3(0.0f, 150.0f + (amount / 60.0f), 0.0f);
        Debug.Log(Head.transform.position);
    }

    #endregion

    #region IPunObservable Implementation

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)   // We own this player, send other player data
        {
            // Send damage
            stream.SendNext(isDamaged);
            stream.SendNext(dmgAmountNet);

            isDamaged = false;
            dmgAmountNet = 0;
        }
        else    // Networked Player
        {
            bool isDamage = (bool)stream.ReceiveNext();
            float damage = (float)stream.ReceiveNext();

            if (isDamage == true)
            {
                Damage(damage);
            }
        }
    }

    #endregion
}
