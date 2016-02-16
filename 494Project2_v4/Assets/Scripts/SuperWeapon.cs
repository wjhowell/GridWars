using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SuperWeapon : MonoBehaviour {

	public List<Sprite> sprites;

	public int row = -1;
	public int col = -1;

	public int owner = -1;
	int charge = 0;

	float frame_interval {
		get { return ((float)(5 - charge)) * 0.2f; }
	}
	public bool at_max_charge {
		get { return charge == 4; }
	}

	float frame_delay = 0f;
	int frame_number = 0;

	static public SuperWeapon instance;

	void Awake() {
		if (instance) {
			throw new UnityException ("Super Weapon already exists.");
		}
		instance = this;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if ((frame_delay -= Time.deltaTime) < 0f) {
			frame_delay = frame_interval;
			frame_number = (frame_number + 1) % sprites.Count;
			GetComponent<SpriteRenderer> ().sprite = sprites [frame_number];
		}
	}

	public void OnNextTurn() {
		if (owner != -1 && charge < 4) {
			++charge;
		} else {
			charge = 0;
		}
	}
}
