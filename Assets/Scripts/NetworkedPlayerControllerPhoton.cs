using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkedPlayerControllerPhoton : MonoBehaviourPunCallbacks
{
    #region Public Variables

    public Transform mTarget;
    public Vector3 mCenter;

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
    private PlayerController EnemyControllerScript;
    private GameObject EnemyHead;

    #endregion

    #region MonoBehaviour Callbacks

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        mCenter = mTarget.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
    }

    /* Photon Functions */
}
