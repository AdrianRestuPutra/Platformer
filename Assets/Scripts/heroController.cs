using UnityEngine;
using System.Collections;

public class heroController : MonoBehaviour {

	//public Vector2 velocity;
	//public float gravity = 50;

	private Rigidbody2D rb;
	public float jumpForce = 10;
	public Vector2 raySource;
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();


	}
	
	// Update is called once per frame
	void Update () {
		//transform.Translate (velocity);
		if(Input.GetButtonDown("Jump") && onGround)
		{
			Debug.Log("ding");
			rb.AddForce(Vector2.up * jumpForce);
		}
		//updateRaySource ();
		//downRaycast ();
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		//Debug.Log ("Ding");
		Debug.Log (coll.contacts [0].normal);
		if (coll.contacts [0].normal == Vector2.up)
			onGround = true;
	}

	void OnCollisionExit2D(Collision2D coll)
	{
		onGround = false;
	}

	void updateRaySource()
	{
		Bounds bound = GetComponent<BoxCollider2D> ().bounds;
		raySource = new Vector2 (bound.center.x, bound.min.y);
	}

	public float rayLength;
	public LayerMask collisionMask;
	public bool onGround = false;
	void downRaycast()
	{
		RaycastHit2D hit = Physics2D.Raycast (raySource, Vector2.down, rayLength, collisionMask);
		Debug.DrawLine (raySource, Vector2.down * rayLength, Color.yellow);
		if (hit) {
			if(hit.distance <= 0.1)
			{
				onGround = true;
			}
			else{
				onGround = false;
			}
			Debug.Log ("hit:" + hit.distance);
		}
		else{
			onGround = false;
		}
	}
}
