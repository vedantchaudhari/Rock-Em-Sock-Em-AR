using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerState : MonoBehaviour
{
    // health
	private float mHealth = 300;

    // black material for when you lose
    public Material DeadMaterial;

    // used to get the head for material
    public GameObject MyHead;

    // get current health
    public float getHealth()
    {
		return mHealth;
	}

    // damageAmount = arm distance converted to damage
    // size = to scale the head movement from damage dealt
	public void damage(float damageAmount, float size)
    {
		mHealth -= damageAmount;
        MyHead.transform.position += new Vector3(0.0f, (damageAmount / 600.0f) * size, 0.0f);
        if(mHealth < 0)
        {
            Dead();
        }
    }

    // switched head color because the player lost
    private void Dead()
    {
        MyHead.GetComponent<Renderer>().material = DeadMaterial;
    }
}