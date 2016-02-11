using UnityEngine;
using System.Collections;

public class cloudCrafter : MonoBehaviour {
	public int numClouds = 10;
	public GameObject cloudPrefab;
	public Vector3 cloudPosMin;
	public Vector3 cloudPosMax;
	public float cloudScaleMin = 1;//delete
	public float cloudScaleMax = 5;//delete
	public float cloudSpeedMult = 0.5f;
	public float[] cloudSpeedRands;

	public GameObject[] cloudInstances;

	void Awake () {
		cloudInstances = new GameObject[numClouds];
		cloudSpeedRands = new float[numClouds];
		GameObject anchor = GameObject.Find ("CloudAnchor");
		GameObject cloud;
		for(int i = 0; i < numClouds; i++){
			cloud = Instantiate (cloudPrefab) as GameObject;
			Vector3 cPos = Vector3.zero;
			cPos.x = Mathf.FloorToInt(Random.Range (cloudPosMin.x, cloudPosMax.x));
			cPos.y = Mathf.FloorToInt(Random.Range (cloudPosMin.y, cloudPosMax.y));
			cPos.z = -0.6f;
			cloud.transform.position = cPos;
			cloud.transform.parent = anchor.transform;
			cloudInstances [i] = cloud;
			cloudSpeedRands [i] = Random.Range (.25f, 1.75f);
		}
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < cloudInstances.Length; i++) {
			GameObject cloud = cloudInstances[i];
			Vector3 cPos = cloud.transform.position;
			cPos.x -= cloudSpeedRands[i] * Time.deltaTime * cloudSpeedMult;
			if(cPos.x <= cloudPosMin.x){
				cPos.x = cloudPosMax.x;
				cPos.y = Mathf.FloorToInt(Random.Range (cloudPosMin.y, cloudPosMax.y));
			}
			cloud.transform.position = cPos;
		}
	}
}
