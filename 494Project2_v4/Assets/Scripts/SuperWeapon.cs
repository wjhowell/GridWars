using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SuperWeapon : MonoBehaviour {

	const int TURNS_TO_CHARGE = 3;

	public List<Sprite> sprites;
	public TextMesh text;

	public int row = -1;
	public int col = -1;

	public int owner = -1;
	int charge = 0;

	float frame_interval {
		get { return ((float)(5 - charge)) * 0.2f; }
	}
	public bool at_max_charge {
		get { return charge == TURNS_TO_CHARGE - 1; }
	}

	float frame_delay = 0f;
	int frame_number = 0;

	static public SuperWeapon instance;

	void Awake() {
		if (instance) {
			throw new UnityException ("Super Weapon already exists.");
		}
		instance = this;

		text = GetComponentInChildren<TextMesh> ();
		text.color = Color.white;
		text.gameObject.SetActive (false);
	}

	float appear_delay = 0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (appear_delay < 1f) {
			appear_delay += Time.deltaTime;
			if (appear_delay >= 1f) {
				GetComponent<Transform> ().localScale = new Vector3 (1f, 1f, 1f);
				appear_delay = 1f;
			} else {
				float u = Mathf.Pow (appear_delay, 3f);
				GetComponent<Transform> ().localScale = new Vector3 (u, u, u);
			}
		}
		if ((frame_delay -= Time.deltaTime) < 0f) {
			frame_delay = frame_interval;
			frame_number = (frame_number + 1) % sprites.Count;
			GetComponent<SpriteRenderer> ().sprite = sprites [frame_number];
		}
	}

	public void OnNextTurn(int whos_turn) {
		if (charge < TURNS_TO_CHARGE - 1) {
			if (owner != -1 && owner == whos_turn) {
				++charge;
				DisplayChargeChange ();
			}
		} else {
			charge = 0;
		}
	}

	public string ActivationText() {
		if (at_max_charge)
			return "Activate Super Weapon!";
		else
			return (TURNS_TO_CHARGE - charge).ToString () + " turns to go!";
	}

	/// <summary>
	/// Only resets owner and charge.
	/// </summary>
	public void ResetCharge() {
		charge = 0;
	}
		
	void DisplayChargeChange() {
		text.text = charge + "/" + TURNS_TO_CHARGE;
		text.gameObject.SetActive (true);
		StartCoroutine (HideChargeChange ());
	}

	IEnumerator HideChargeChange(){
		yield return new WaitForSeconds(0.5f);
		text.color = Color.black;
		yield return new WaitForSeconds(0.5f);
		text.color = Color.white;
		yield return new WaitForSeconds(0.5f);
		text.gameObject.SetActive (false);
	}
}
