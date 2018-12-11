using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punching : MonoBehaviour
{
    // hold onto main body for enemy head game object
    public GameObject mainBody;

    // used to determine which hand punched
    public bool right;
    private void OnTriggerEnter(Collider other)
    {
        // check if hit head and not just opponent body
        if (mainBody.GetComponent<PlayerController>().EnemyHead.GetComponent<Collider>() == other)
        {
            // check whcih hand hit to determine correct arm distance for damage dealt
            if (right)
            {
                mainBody.GetComponent<PlayerController>().hit(mainBody.GetComponent<PlayerController>().RightArmDistance);
            }
            else
            {
                mainBody.GetComponent<PlayerController>().hit(mainBody.GetComponent<PlayerController>().LeftArmDistance);
            }
        }
    }
}