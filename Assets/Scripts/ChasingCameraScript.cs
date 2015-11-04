using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChasingCameraScript : MonoBehaviour {
	private GameObject target;

	public float treshold = 3f;
	// Use this for initialization
	void Start () {
//		CanvasScaler panelScalar = GetComponent<CanvasScaler>();
//		RectTransform panelRect = GameObject.Find ("Panel").GetComponent<RectTransform> ();
//
//		panelScalar.referenceResolution = new Vector2(1024f, 768f);
//
//		float scaleFactor = GetComponent<Canvas>().scaleFactor;
//		panelRect.sizeDelta = new Vector2(Screen.width / scaleFactor * 1, 768f);
//
//		print (Camera.main.aspect + "");

		target = GameObject.Find("Hero");
	}
	
	// Update is called once per frame
	void Update () {
		UpdateCameraPosition ();
	}

	private void UpdateCameraPosition () {
		if (target != null) {
			if (Mathf.Abs(target.transform.position.x - transform.position.x) > treshold || Mathf.Abs((target.transform.position.y - 5f) - transform.position.y) > treshold - 2f) {
				transform.position = Vector3.Lerp (transform.position, new Vector3(target.transform.position.x, (target.transform.position.y - 5f), -10f), Time.deltaTime * 3);
//				transform.position = target.transform.position;
			}
		} else target = GameObject.Find("Hero");
	}
}
