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
	}

	void FixedUpdate () {
//		GravityEffect ();
		UpdateRaySource ();
		CheckGround ();
		MovePlayer ();
	}
	
	// Update is called once per frame
	void Update () {
		bool moveLeft = Input.GetKey (KeyCode.A);
		bool moveRight = Input.GetKey (KeyCode.D);
		bool jump = Input.GetKeyDown (KeyCode.Space) || (Input.GetButtonDown("Fire1") && Input.GetAxis("Vertical") >= 0);
		bool moveDown = Input.GetKeyDown (KeyCode.S) || (Input.GetButtonDown("Fire1") && Input.GetAxis("Vertical") < 0);
		bool shoot = Input.GetKey(KeyCode.O) || Input.GetButton("Fire3");
		float controllerHorizontal = Input.GetAxis("Horizontal");

		movement = Vector3.zero;
		if (moveLeft || controllerHorizontal < 0) {
			movement.x = -1f;
			if(isFacingRight) {
				isFacingRight = false;
				transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
			}
		}
		if (moveRight || controllerHorizontal > 0) {
			movement.x = 1f;
			if(!isFacingRight) {
				isFacingRight = true;
				transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			}
		}
		if (jump && onGround) {
			print("jump");
			playerRigidBody.AddForce(Vector2.up * jumpForce * 200);
		}

		if (moveDown) {
			GameObject platform = GetPlatform();
			if(IsUpperPlatform(platform))
				StartCoroutine (DisableCollider());
		}

		if (shoot) {
			StartCoroutine(SpawnBullet());
		}
	}

	private void GravityEffect () {
//		transform.position += Vector3.down * gravity * Time.deltaTime;
	}

	private void MovePlayer () {
		if (movement != Vector3.zero) {
			animator.SetBool("IsMoving", true);
			transform.position += movement * speed * Time.deltaTime * 3f;
		} else {
			animator.SetBool("IsMoving", false);
		}
	}

	void UpdateRaySource () {
//		Bounds bound = GetComponent<PolygonCollider2D> ().bounds;
		Bounds bound = GetComponent<BoxCollider2D> ().bounds;
		raySource = new Vector2 (bound.center.x, bound.min.y);
	}

	private void CheckGround () {
		RaycastHit2D hit = Physics2D.Raycast (raySource, Vector2.down, rayLength, collisionMask);
		Debug.DrawLine (raySource, raySource + (Vector2.down * rayLength), Color.yellow);
		if (hit) {
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

}
