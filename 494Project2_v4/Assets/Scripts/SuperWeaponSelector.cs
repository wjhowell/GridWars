using UnityEngine;
using System.Collections;

/// <summary>
/// On screen object that follows user's mouse that indicates targets to launch
/// a "super-attack". This highly imbalenced attack allows the player who 
/// posseses it to quickly reach the win condition, will still giving other
/// players a chance to take the super weapon back for themselves.
/// </summary>
public class SuperWeaponSelector : MonoBehaviour {

	public int owner = 0;

	public bool is_active = false;

	// Use this for initialization
	void Start () {
		Color c = GetComponent<SpriteRenderer> ().color;
		c = play_data.OwnerIntToColor (owner);
		c.a = 0.3f;
		GetComponent<SpriteRenderer> ().color = c;
	}
	
	// Update is called once per frame
	void Update () {
		if (!is_active)
			return;

		// special thanks to "save"
		// url: http://answers.unity3d.com/questions/123647/how-to-detect-mouse-movement-as-an-input.html
		if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0) {
			OnMouseMove ();
		}
		if (Input.GetMouseButtonDown (0) || Input.GetMouseButtonDown (1) || Input.GetMouseButtonDown (2)) {
			OnMousePress ();
		}

		// special modification to linear interpolation that prevents 1 jumping to 0 immediately
		// this on "continuously" approaches 0 from 1 rather than "jumping"
		float u = Mathf.Abs ((Time.time % 1f - 0.5f) * 2f);
		u = u * u;

		Color c = GetComponent<SpriteRenderer> ().color;
		c.a = 0.2f + u * 0.4f;
		GetComponent<SpriteRenderer> ().color = c;
	}

	/// <summary>
	/// Raises the mouse move event.
	/// Called in Update() method.
	/// Selector follows user's mouse position.
	/// </summary>
	void OnMouseMove() {
		// get the tile location from the mouse position
		Vector3 pos = Input.mousePosition;
		pos.z = 0f;
		pos = Camera.main.ScreenToWorldPoint (pos);
		pos.x = Mathf.Round (pos.x);
		pos.y = Mathf.Round (pos.y);

		// filter out, out of range positions
		if (pos.x < 0f || pos.x >= (float)play_data.instance.tiles.GetLength (0)
		    || pos.y < 0f || pos.y >= (float)play_data.instance.tiles.GetLength (1))
			return;

		pos.z = 0f;

		// set position of selector to match
		GetComponent<Transform>().position = pos;
	}

	/// <summary>
	/// Raises the mouse press event.
	/// Called in Update() method.
	/// Fires the super weapon!
	/// </summary>
	void OnMousePress() {
		int target_col = (int)GetComponent<Transform> ().position.x;
		int target_row = (int)GetComponent<Transform> ().position.y;
		play_data.instance.DoSuperAttack (target_col, target_row);
		is_active = false;
		SuperWeapon.instance.ResetCharge ();
		Destroy (gameObject);
	}
}
