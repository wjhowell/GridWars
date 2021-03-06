﻿using UnityEngine;
using System.Collections;

public class linearInterp : MonoBehaviour {
	public float u, pow;
	public Vector3 p0, p1, p2;

	// Update is called once per frame
	void Update () {
		if (u <= .99999) {
			u = .1f * Time.time % 1.0f;
			u = 1 - Mathf.Pow (1 - u, pow);
			Vector3 p01 = (1 - u) * p0 + u * p1;
			this.transform.position = p01;
		}
	}
}
