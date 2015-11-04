using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
	public float gravity = 2;
	public float speed = 2;
	public float jumpForce = 10;

	public float rayLength = 1;
	public LayerMask collisionMask;
	public GameObject bulletSpawnPoint;
	public GameObject bulletPrefab;
	public float shootDelay = 0.2f;

	private Vector3 movement;
	private Animator animator;
	private Rigidbody2D playerRigidBody;
	private BoxCollider2D playerCollider;

	private Vector2 raySource;
	private bool isFacingRight;
	private bool onGround;
	private bool onUpperPlatform;
	private bool collideWithStairs, onStairs;
	private bool canShoot;


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

		animator = GetComponent<Animator> ();
		playerRigidBody = GetComponent<Rigidbody2D> ();
		playerCollider = GetComponent<BoxCollider2D> ();
		isFacingRight = true;
		canShoot = true;
		collideWithStairs = false;
		onStairs = false;
	}

	void FixedUpdate () {
		UpdateRaySource ();
		CheckGround ();
		MovePlayer ();
	}
	
	// Update is called once per frame
	void Update () {
		bool moveLeft = Input.GetKey (KeyCode.A);
		bool moveRight = Input.GetKey (KeyCode.D);
		bool jump = !Input.GetKey (KeyCode.S) && Input.GetKeyDown (KeyCode.Space) || (Input.GetButtonDown("Fire1") && Input.GetAxis("Vertical") >= 0f);
		bool jumpDown = Input.GetKey (KeyCode.S) && Input.GetKeyDown (KeyCode.Space) || (Input.GetButtonDown("Fire1") && Input.GetAxis("Vertical") < -0.3f);
		bool moveDown = Input.GetKey (KeyCode.S) || Input.GetAxis("Vertical") < 0f;
		bool moveUp = Input.GetKey (KeyCode.W) || Input.GetAxis("Vertical") > 0f;
		bool shoot = Input.GetKey (KeyCode.O) || Input.GetButton("Fire3");
		bool use = Input.GetKeyDown (KeyCode.E) || Input.GetButtonDown("Fire2");
		float controllerHorizontal = Input.GetAxis("Horizontal");

		movement = Vector3.zero;
		if (moveLeft || controllerHorizontal < 0) {
			if(isFacingRight) {
				isFacingRight = false;
				transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
			} else movement.x = -1f;
		}
		if (moveRight || controllerHorizontal > 0) {
			if(!isFacingRight) {
				isFacingRight = true;
				transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			} else movement.x = 1f;

		}
		if (jump) {
			if(onStairs) {
				collideWithStairs = false;
				onStairs = false;
				playerRigidBody.isKinematic = false;
				playerRigidBody.AddForce(Vector2.up * jumpForce * 100);
			} else if(onGround){
				playerRigidBody.AddForce(Vector2.up * jumpForce * 200);
			}
		}

		if (jumpDown) {
			GameObject platform = GetPlatform();
			if(IsUpperPlatform(platform))
				StartCoroutine (DisableCollider());
		}

		if (use) {
			if(collideWithStairs) {
				onStairs = true;
				playerRigidBody.isKinematic = true;
			}
		}

		if (moveDown && onStairs) {
//			if(!onStairs) {
//				// TODO set animation stair
//				onStairs = true;
//				playerRigidBody.isKinematic = true;
//			}
			movement.y = -1f;
		}

		if (moveUp && onStairs) {
//			if(!onStairs) {
//				// TODO set animation stair
//				onStairs = true;
//				playerRigidBody.isKinematic = true;
//			}
			movement.y = 1f;
		}

		if (shoot) {
			animator.SetBool("IsShooting", true);
			StartCoroutine(SpawnBullet());
		} else animator.SetBool("IsShooting", false);
	}

	private void MovePlayer () {
		playerRigidBody.velocity = Vector2.ClampMagnitude (playerRigidBody.velocity, 60f);

		if (movement != Vector3.zero) {
			animator.SetBool("IsMoving", true);
			transform.position += movement * speed * Time.deltaTime * 3f;
		} else {
			animator.SetBool("IsMoving", false);
		}
	}

	void UpdateRaySource () {
		Bounds bound = GetComponent<BoxCollider2D> ().bounds;
		raySource = new Vector2 (bound.center.x, bound.min.y);
	}

	private void CheckGround () {
		RaycastHit2D hit = Physics2D.Raycast (raySource, Vector2.down, rayLength, collisionMask);
		Debug.DrawLine (raySource, raySource + (Vector2.down * rayLength), Color.yellow);
		if (hit && playerRigidBody.velocity.y <= 0) {
			onGround = true;
			animator.SetBool ("IsFalling", false);
		} else {
			onGround = false;
			animator.SetBool("IsFalling", true);
		}
	}

	private GameObject GetPlatform () {
		RaycastHit2D hit = Physics2D.Raycast (raySource, Vector2.down, rayLength, collisionMask);
		Debug.DrawLine (raySource, raySource + (Vector2.down * rayLength), Color.yellow);
		if (hit) {
			return hit.collider.gameObject;
		} else
			return null;
	}

	private bool IsUpperPlatform (GameObject obj) {
		if (obj != null)
			return (obj.GetComponent<PlatformEffector2D> () != null);
		else
			return false;
	}

	private IEnumerator DisableCollider () {
		BoxCollider2D playerCollider = GetComponent<BoxCollider2D> ();

		playerCollider.enabled = false;
		yield return new WaitForSeconds(0.2f);
		playerCollider.enabled = true;
	}

	private IEnumerator SpawnBullet () {
		if (canShoot) {
			canShoot = false;
			if (isFacingRight)
				Instantiate (bulletPrefab, bulletSpawnPoint.transform.position, Quaternion.identity);
			else 
				Instantiate (bulletPrefab, bulletSpawnPoint.transform.position, Quaternion.Euler (0f, 180f, 0f));

			yield return new WaitForSeconds (shootDelay);
			canShoot = true;
		}
	}

	void OnTriggerEnter2D (Collider2D coll) {
		GameObject obj = coll.gameObject;
		if (obj.tag == "EnemyBullet") {
//			health--;
			Destroy(obj);
		}

		if (obj.tag == "Stairs") {
			collideWithStairs = true;
		}
	}

	void OnTriggerExit2D (Collider2D coll) {
		GameObject obj = coll.gameObject;
		if (obj.tag == "Stairs") {
			collideWithStairs = false;
			onStairs = false;
			playerRigidBody.isKinematic = false;
		}
	}
}
