using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour {

	public PlatformController parentPlatform;
	public GameObject target;
	public float damage;
	public float speed;
	public float health;
	public bool retargets = false;

	Rigidbody2D physicsBody;
	int parentPlatformID;

	// Use this for initialization
	void Start () {
		parentPlatformID = parentPlatform.gameObject.GetInstanceID();
		physicsBody = gameObject.GetComponent<Rigidbody2D>();

		transform.Translate(new Vector3(0, 0, -2), Space.World);
	}

	// Update is called once per frame
	void Update () {
		if (!target) {
			if (retargets) {
				Retarget(parentPlatform.GetTargetPlatform().RequestBuildingTarget());
			} else {
				Destroy(gameObject);
			}
		} else {
			Vector3 relativePosition = transform.position - target.transform.position;
			Quaternion rotation = Quaternion.LookRotation(relativePosition, Vector3.forward);
			rotation.x = 0f;
			rotation.y = 0f;

			transform.rotation = rotation;
		}
	}

	void FixedUpdate() {
		Vector3 directionVector = target.transform.position - transform.position;
		Vector2 directionVector2D;

		directionVector.Normalize();
		directionVector *= speed;

		directionVector2D = new Vector2(directionVector.x, directionVector.y);

		physicsBody.AddForce(directionVector2D);
	}

	void OnTriggerEnter2D (Collider2D col) {
		BuildingController building = col.gameObject.GetComponent<BuildingController>();
		PlatformController platform = col.gameObject.GetComponent<PlatformController>();

		if (building && building.parentPlatformID != parentPlatformID) {
			col.gameObject.SendMessage("Damage", damage);
			Destroy(gameObject);
		} else if (platform && platform.gameObject.GetInstanceID() != parentPlatformID) {
			if (Random.value < platform.GetLeakChance(speed)) {
				Debug.Log(platform.GetLeakChance(speed));
				Debug.Log("Leak!");
			} else {
				col.gameObject.SendMessage("DamageShield", damage);
				Destroy(gameObject);
			}
		}
	}

	public void Retarget (GameObject newTarget) {
		target = newTarget;
		Vector3 relativePosition = transform.position - target.transform.position;
		Quaternion rotation = Quaternion.LookRotation(relativePosition, Vector3.forward);
		rotation.x = 0f;
		rotation.y = 0f;

		transform.rotation = rotation;
	}

	public void SetPlatform(PlatformController platform) {
		parentPlatform = platform;
	}
}
