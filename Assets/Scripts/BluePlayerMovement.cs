using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BluePlayerMovement : MonoBehaviour
{

    public Transform mTarget;

    private Vector3 mCenter;
    private float mRadius = 4.0f;

    public GameObject RightArmPunch;
    public GameObject LeftArmPunch;

    private float ArmSpeed = 70.0f;

    private bool LeftPunch = true;
    private bool RightPunch = true;

    // Use this for initialization
    void Start()
    {
        mCenter = mTarget.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        getInput();
    }

    /* PRIVATE FUNCTIONS */
    private void getInput()
    {
        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            if (Input.GetKey(KeyCode.D))
                Rotate(true);
            else if (Input.GetKey(KeyCode.A))
            {
                Rotate(false);
            }

            if (Input.GetKey(KeyCode.Q)) // Left punch
            {
                if (LeftPunch)
                {
                    Punch(LeftArmPunch);
                    LeftPunch = false;
                }
            }
            else if (!LeftPunch)
            {
                LeftPunch = true;
                ResetPunch(LeftArmPunch);
            }

            if (Input.GetKey(KeyCode.E)) // Right punch
            {
                if (RightPunch)
                {
                    Punch(RightArmPunch);
                    RightPunch = false;
                }
            }
            else if (!RightPunch)
            {
                RightPunch = true;
                ResetPunch(RightArmPunch);
            }
        }
        else
        {   // SETUP FOR MOBILE CONTROLS

        }
    }

    private void Punch(GameObject fist)
    {
        fist.transform.position += fist.transform.forward * 1.0f;
    }

    private void ResetPunch(GameObject fist)
    {
        fist.transform.position -= fist.transform.forward * 1.0f;
    }


    private Vector2 PointOnCircle(float angle)
    {
        float x = (mRadius * Mathf.Cos(angle * Mathf.PI / 180.0f) + mCenter.x);
        float y = (mRadius * Mathf.Cos(angle * Mathf.PI / 180.0f) + mCenter.y);

        return new Vector2(x, y);
    }

    // True for right, False for left
    private void Rotate(bool direction)
    {
        if (direction)
            transform.RotateAround(mCenter, new Vector3(0, 0.5f, 0), 180 * Time.deltaTime);
        else
            transform.RotateAround(mCenter, -new Vector3(0, 0.5f, 0), 180 * Time.deltaTime);
    }
}
