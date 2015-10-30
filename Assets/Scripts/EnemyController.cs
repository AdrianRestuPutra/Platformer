using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {
	public int health = 5;
	public float speed = 3;
	private GameObject target;

	// Use this for initialization
	void Start () {
		target = GameObject.Find("Player");
	}

	void FixedUpdate () {
		if(target != null)
			MoveToTarget ();
//		else print ("target exists");
	}

	// Update is called once per frame
	void Update () {
		if (target == null)
			GetTarget ();
		CheckHealth ();
	}

	void OnTriggerEnter2D (Collider2D coll) {
		GameObject obj = coll.gameObject;
		if (obj.tag == "Weapon") {
			health--;
			Destroy(obj);
		}
	}

	private void CheckHealth () {
		if (health <= 0) 
			Destroy (gameObject);
	}

	private void GetTarget () {
		target = GameObject.Find("Player");
	}

	private void MoveToTarget () {
		transform.position = Vector3.MoveTowards (transform.position, target.transform.position, speed * Time.deltaTime);
	}
}
