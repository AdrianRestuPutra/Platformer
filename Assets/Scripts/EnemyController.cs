using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {
	public int health = 5;
	public float speed = 3;
	public float noticeArea = 50f;
	public GameObject bulletSpawnPoint;
	public GameObject bulletPrefab;
	public float noticeCoolDown = 10f;
	public float delayStartShoot = 2f;
	public float shootDelay = 0.2f;

	private GameObject target;

	private float currentTimer;
	private Vector3 movement;
	private bool alert;
	private bool isFacingRight;
	private bool canShoot;

	private Vector2 raySource;

	// Use this for initialization
	void Start () {
		target = GameObject.Find("Hero");
		alert = false;
		isFacingRight = true;
		canShoot = true;
	}

	void FixedUpdate () {
		if(target != null)
			MoveToTarget ();
	}

	// Update is called once per frame
	void Update () {
		if (target == null)
			GetTarget ();
		else {
			CheckHealth ();
//			CheckLineOfSight ();
		}
	}

	void OnTriggerEnter2D (Collider2D coll) {
		GameObject obj = coll.gameObject;
		if (obj.tag == "PlayerBullet") {
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
	
//	private void CheckLineOfSight () {
//		raySource = new Vector2 (transform.position.x, transform.position.y);
//		RaycastHit2D hit = Physics2D.Raycast (raySource, 
//		Debug.DrawLine (raySource, target.transform.position, Color.yellow);
////		if (hit) {
////		} else {
////		}
//	}

	private void MoveToTarget () {
		float distanceToTarget = Vector2.Distance (target.transform.position, transform.position);
		if (distanceToTarget < noticeArea && (Mathf.Abs (target.transform.position.y - transform.position.y) < 5)) {
			if(!alert) {
				alert = true;
				currentTimer = delayStartShoot;
				noticeArea = 60f;
			}
			movement = Vector3.zero;

			if (target.transform.position.x < transform.position.x) {
				// Move Left
				movement.x = -1f;
				if (isFacingRight) {
					isFacingRight = false;
					transform.localRotation = Quaternion.Euler (0f, 180f, 0f);
				}
			} else {
				// Move Right
				movement.x = 1f;
				if (!isFacingRight) {
					isFacingRight = true;
					transform.localRotation = Quaternion.Euler (0f, 0f, 0f);
				}
			}

			if (distanceToTarget <= 30f) {
				currentTimer -= Time.deltaTime;
				if (currentTimer <= 0) 
					StartCoroutine (SpawnBullet ());
			} else {
				transform.position += movement * speed * Time.deltaTime;
			}
		} else {
			currentTimer = 1f;
		}
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
