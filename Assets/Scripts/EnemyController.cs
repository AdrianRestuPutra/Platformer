using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {
	private enum EnemyState {
		idle,
		alert,
		calmDown
	}
	public int health = 5;
	public float speed = 3;
	public float noticeArea = 50f;
	public GameObject bulletSpawnPoint;
	public GameObject bulletPrefab;
	public float noticeCoolDown = 10f;
	public float delayStartShoot = 1f;
	public float shootDelay = 0.2f;

	private GameObject target;

	private float currentTimer;
	private float calmTimer;

	private EnemyState state;
	private Vector3 movement;
	private bool alert;
	private bool isFacingRight;
	private bool isTurningAround;
	private bool canShoot;

	private Vector2 raySource;
	private Vector3 spawnPoint;

	// Use this for initialization
	void Start () {
		target = GameObject.Find("Hero");
		alert = false;
		isFacingRight = true;
		canShoot = true;
		isTurningAround = false;
		state = EnemyState.idle;

		spawnPoint = transform.position;
	}

	void FixedUpdate () {
		if (target != null) {
			movement = Vector3.zero;

			switch(state) {
			case EnemyState.idle :
				Idling ();
				break;

			case EnemyState.alert :
				MoveToTarget ();
				break;

			case EnemyState.calmDown :
				CalmDown ();
				break;
			}

			MoveEnemy ();
		}
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
			SetAlert ();
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

	private void Idling () {
		if (!isTurningAround) {
			if (isFacingRight) {
				if (transform.position.x - spawnPoint.x > 20f)
					StartCoroutine (TurnAround ());
				else
					movement.x = 1f;

			} else {
				if (transform.position.x - spawnPoint.x < -20f)
					StartCoroutine (TurnAround ());
				else
					movement.x = -1f;

			}
		}
	}

	private void SetAlert () {
		if (!alert) {
			alert = true;
			currentTimer = delayStartShoot;
		}
		state = EnemyState.alert;
	}

	private void SetCalm () {
		state = EnemyState.calmDown;
		currentTimer = 3f;
	}

	private void MoveToTarget () {
		float distanceToTarget = Vector2.Distance (target.transform.position, transform.position);
		if (distanceToTarget < noticeArea && (Mathf.Abs (target.transform.position.y - transform.position.y) < 5)) {
			calmTimer = 0f;

			if (target.transform.position.x < transform.position.x) {
				// Moving Left and rotate sprite to left side
				movement.x = -1f;
				if (isFacingRight) {
					isFacingRight = false;
					transform.localRotation = Quaternion.Euler (0f, 180f, 0f);
				}
			} else {
				// Moving Right and rotate sprite to right side 
				movement.x = 1f;
				if (!isFacingRight) {
					isFacingRight = true;
					transform.localRotation = Quaternion.Euler (0f, 0f, 0f);
				}
			}

			if (distanceToTarget <= 30f) {
				movement.x = 0f;
				currentTimer -= Time.deltaTime;
				if (currentTimer <= 0) {

					StartCoroutine (SpawnBullet ());
				}
			} else {
				transform.position += movement * speed * Time.deltaTime;
			}
		} else {
			currentTimer = delayStartShoot;

			calmTimer += Time.deltaTime;
			if(calmTimer >= 3f) SetCalm();
		}
	}

	private void CalmDown () {
		currentTimer -= Time.deltaTime;

		if (currentTimer <= 0) {
			alert = false;
			state = EnemyState.idle;
		}
	}

	private void MoveEnemy () {
		transform.position += movement * speed * Time.deltaTime;

	}

	private IEnumerator TurnAround () {
		isTurningAround = true;

		yield return new WaitForSeconds (2f);
		isFacingRight = !isFacingRight;
		if(isFacingRight == true) transform.localRotation = Quaternion.Euler (0f, 0f, 0f);
		else transform.localRotation = Quaternion.Euler (0f, 180f, 0f);

		isTurningAround = false;
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

	private IEnumerator ReturnToIdle () {
		yield return new WaitForSeconds (2f);
		state = EnemyState.idle;
	}
}
