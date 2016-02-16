using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class buttonHover : MonoBehaviour {

	void OnMouseOver()
	{
		Hud.instance.updateAction(gameObject.name);
	}

}
