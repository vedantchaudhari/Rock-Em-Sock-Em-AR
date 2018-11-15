using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerState : MonoBehaviour {

	public Text mHealthText;
	private int mHealth = 100;
	// Use this for initialization
	void Start () {
		mHealthText.rectTransform.anchorMin = new Vector2(0, 1);
		mHealthText.rectTransform.anchorMax = new Vector2(0, 1);
		mHealthText.rectTransform.pivot = new Vector2(0, 1);
		mHealthText.alignment = TextAnchor.LowerCenter;
		mHealthText.text = mHealth.ToString();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	int getHealth() {
		return mHealth;
	}

	void damage(int amount) {
		mHealth -= amount;
	}
}
