using UnityEngine;
using System.Collections;

public class ChasingCameraScript : MonoBehaviour {
	private GameObject target;

	public float treshold = 6f;
	// Use this for initialization
	void Start () {
		target = GameObject.Find("Hero");
	}
	
	// Update is called once per frame
	void Update () {
		UpdateCameraPosition ();
	}

	private void UpdateCameraPosition () {
		if (target != null) {
			if (Mathf.Abs(target.transform.position.x - transform.position.x) > treshold || Mathf.Abs(target.transform.position.y - transform.position.y) > treshold) {
				transform.position = Vector3.Lerp (transform.position, new Vector3(target.transform.position.x, target.transform.position.y, -10f), Time.deltaTime);
//				transform.position = target.transform.position;
			}
		} else target = GameObject.Find("Hero");
	}
}
