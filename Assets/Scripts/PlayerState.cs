﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerState : MonoBehaviour {

	public Text mHealthText;
	private float mHealth = 300;

	// Use this for initialization
	void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

	float getHealth()
    {
		return mHealth;
	}

	public void damage(float amount)
    {
		mHealth -= amount;
	}
}
