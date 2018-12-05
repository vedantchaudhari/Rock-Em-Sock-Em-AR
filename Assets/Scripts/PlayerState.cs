using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class PlayerState : MonoBehaviourPunCallbacks {

	private float mHealth = 300;

    public Material DeadMaterial;

    public GameObject MyHead;

    public float getHealth()
    {
		return mHealth;
	}

	public void damage(float damageAmount)
    {
		mHealth -= damageAmount;
        MyHead.transform.position += new Vector3(0.0f, damageAmount / 600.0f, 0.0f);
        //if (mHealth < 0.0f)
        //{
        //    Dead();
        //}
    }

    private void Dead()
    {
        MyHead.GetComponent<Renderer>().material = DeadMaterial;
    }
}