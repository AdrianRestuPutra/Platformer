using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour {
	public float speed = 10;
	public float maxDistance = 10;

	private Vector2 startPoint;

	// Use this for initialization
	void Start () {
		startPoint = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		CheckDistance ();
		transform.position += transform.right * speed * Time.deltaTime * 20f;
	}

	private void CheckDistance () {
		if (Vector2.Distance (transform.position, startPoint) >= maxDistance)
			Destroy (gameObject);
	}
}
