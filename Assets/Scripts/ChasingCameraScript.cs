using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChasingCameraScript : MonoBehaviour {
	private GameObject target;

	public float treshold = 3f;
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
			if (Mathf.Abs(target.transform.position.x - transform.position.x) > treshold || Mathf.Abs(target.transform.position.y - transform.position.y) > treshold - 2f) {
				transform.position = Vector3.Lerp (transform.position, new Vector3(target.transform.position.x, target.transform.position.y, -10f), Time.deltaTime * 3);
//				transform.position = target.transform.position;
			}
		} else target = GameObject.Find("Hero");
	}
}
