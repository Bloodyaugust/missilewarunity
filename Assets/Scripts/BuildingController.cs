using UnityEngine;
using System.Collections;

public class BuildingController : MonoBehaviour {

	public PlatformController parentPlatform;
	public TileController tile;
	public GameObject[] buildableProjectilePrefabs;
	public GameObject[] buildingUpgradePrefabs;
	public GameObject[] purchasables;
	public GameObject[] buildQueue;
	public GameObject baseProjectilePrefab;
	public GameObject uiBuildButton;
	public string buildingType = "booster";
	public float cost = 1;
	public float health = 1;
	public float actionInterval = 5;
	public float shieldRegenAmount = 1;
	public float shieldMaxAmount = 0;
	public float energyGenerationAmount = 1;
	public float energyConsumption = 1;
	public float fireInterval = 1;
	public int buildQueueMax = 5;
	public int currentFireIndex = 0;
	public int parentPlatformID;

	public string state = "idle";
	public float timeToAction = 0;
	public float timeToFire = 0;

	bool powered = false;

	// Use this for initialization
	void Start () {
		parentPlatform = transform.parent.GetComponent<PlatformController>();
		parentPlatformID = parentPlatform.gameObject.GetInstanceID();

		if (buildingType == "silo") {
			buildQueue = new GameObject[buildQueueMax];
			buildQueue[0] = baseProjectilePrefab;
		} else if (buildingType == "generator") {
			powered = true;
		}

		parentPlatform.ChangeMaxShield(shieldMaxAmount);

		Vector4 materialCenter = transform.position;
		materialCenter.z = 0;

		GetComponent<Renderer>().material.SetVector("_Center", materialCenter);
	}

	// Update is called once per frame
	void Update () {
		if (buildingType == "silo") {
			if (state == "firing") {
				timeToFire -= Time.deltaTime;
			}
		}
		if (buildingType != "generator") {
			powered = parentPlatform.DrainEnergy(energyConsumption);
		}

		if (powered && state != "firing") {
			timeToAction -= Time.deltaTime;
		}

		if (timeToAction <= 0 && powered) {
			timeToAction = actionInterval;

			switch (buildingType) {
				case "booster":
					transform.parent.SendMessage("AddShield", shieldRegenAmount);
					break;

				case "generator":
					transform.parent.SendMessage("AddEnergy", energyGenerationAmount);
					break;

				case "silo":
					state = "firing";
					break;

				default:
					break;
			}
		}

		if (buildingType == "silo") {
			if (state == "firing" && timeToFire <= 0) {
				GameObject newProjectile = Instantiate(buildQueue[currentFireIndex], transform.position, transform.rotation) as GameObject;
				newProjectile.SendMessage("SetPlatform", parentPlatform);
				newProjectile.SendMessage("Retarget", parentPlatform.GetTargetPlatform().RequestBuildingTarget());
				timeToFire = fireInterval;
				buildQueue[currentFireIndex] = null;

				currentFireIndex++;
				if (!buildQueue[currentFireIndex]) {
					currentFireIndex = 0;
					state = "idle";
					buildQueue[0] = baseProjectilePrefab;
				}
			}
		}

		if (health <= 0) {
			tile.building = null;

			if (buildingType == "command") {
				transform.parent.SendMessage("CommandDeath");
			}

			parentPlatform.ChangeMaxShield(-shieldMaxAmount);

			Destroy(gameObject);
		}
	}

	void OnMouseUp () {
		if (parentPlatform.isPlayer) {
			parentPlatform.SetActiveTile(tile);
		}
	}

	public void Damage(float amount) {
		health -= amount;
	}
}
