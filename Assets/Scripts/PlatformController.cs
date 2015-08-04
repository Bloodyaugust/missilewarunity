using UnityEngine;
using System.Collections;

public class PlatformController : MonoBehaviour {

	public GameObject[] buildingPrefabs;
	public GameObject[] tiles;
	public TileController[] tileControllers;
	public GameObject tilePrefab;
	public GameObject commandPrefab;
	public PlatformController targetPlatform;
	public TileController activeTile;
	public GameUIHandler uiHandler;
	public float health = 100;
	public float shield = 100;
	public float shieldMax = 100;
	public float energy = 10;
	public float baseEnergyInterval = 5;
	public float baseEnergyGain = 10;
	public float baseShieldInterval = 1;
	public float baseShieldGain = 2;
	public float shieldFullRechargeInterval = 10;
	public float guaranteedLeakSpeed = 800;
	public float shieldLeakModifier = 0.5f;
	public int team = 0;
	public int owner = 0;
	public bool shieldActive = true;
	public bool isPlayer = true;
	public bool isAlive = true;

	GameObject shieldObject;
	SpriteRenderer shieldRenderer;
	CircleCollider2D shieldCollider;
	string[] buildButtons;
	float shieldRadius;
	float timeToEnergyGain = 0;
	float timeToShieldGain = 0;
	float timeToShieldFullRecharge = 0;

	// Use this for initialization
	void Start () {
		buildButtons = new string[buildingPrefabs.Length];

		for (int i = 0; i < buildingPrefabs.Length; i++) {
			buildButtons[i] = buildingPrefabs[i].name;
		}

		shieldObject = transform.Find("Shield").gameObject;
		shieldCollider = GetComponent<CircleCollider2D>();
		shieldRenderer = shieldObject.GetComponent<SpriteRenderer>();
	}

	void Construct (int[] cells) {
		TileController commandTile;
		GameObject newTile, commandBuilding;
		Vector3 commandBuildingLocation;
		tiles = new GameObject[cells.Length];
		tileControllers = new TileController[cells.Length];
		float furthestTileDistance = 0;
		float newScale;
		int side = (int)Mathf.Sqrt(cells.Length);
		int furthestTile = 0;

		for (int i = 0; i < cells.Length; i++) {
			if (cells[i] > 0 || i == cells.Length / 2) {
				newTile = Instantiate(tilePrefab, transform.position + new Vector3((i % side) * 64f - (64f * (float)side) / 2 + 32, Mathf.Floor(i / side) * 64f - (64f * (float)side) / 2 + 32, 0), transform.rotation) as GameObject;
				newTile.transform.parent = transform;

				tiles[i] = newTile;
				tileControllers[i] = newTile.GetComponent<TileController>();

				furthestTile = newTile.transform.localPosition.sqrMagnitude > furthestTileDistance ? i : furthestTile;
				furthestTileDistance = (furthestTile == i) ? newTile.transform.localPosition.sqrMagnitude : furthestTileDistance;
			}

			if (i == cells.Length / 2) {
				commandTile = tiles[i].GetComponent<TileController>();

				commandBuildingLocation = commandTile.transform.position;
				commandBuildingLocation.z = -1;
				commandBuilding = Instantiate(commandPrefab, commandBuildingLocation, commandTile.transform.rotation) as GameObject;
				commandBuilding.transform.parent = transform;
				commandTile.building = commandBuilding.GetComponent<BuildingController>();
				commandTile.building.tile = commandTile;
			}
		}

		shieldRadius = tiles[furthestTile].transform.localPosition.magnitude + 64f;

		newScale = shieldRadius / 256f;
		transform.Find("Shield").transform.localScale = new Vector3(newScale, newScale, 1);
		GetComponent<CircleCollider2D>().radius = shieldRadius;
	}

	// Update is called once per frame
	void Update () {
		timeToEnergyGain -= Time.deltaTime;
		timeToShieldGain -= Time.deltaTime;

		if (isAlive) {
			if (!targetPlatform) {
				RetargetPlatform();
			}

			if (shieldActive) {
				if (shield <= 0) {
					shield = 0;
					shieldActive = false;

					timeToShieldFullRecharge = shieldFullRechargeInterval;
					shieldObject.SetActive(false);
					shieldCollider.enabled = false;
				} else {
					shieldRenderer.color = new Color(1f, 1f, 1f, (shield / shieldMax));
				}

				if (timeToShieldGain <= 0) {
					timeToShieldGain = baseShieldInterval;
					AddShield(baseShieldGain);
				}
			} else {
				timeToShieldFullRecharge -= Time.deltaTime;

				if (timeToShieldFullRecharge <= 0) {
					timeToShieldFullRecharge = 0;
					shieldActive = true;
					shield = shieldMax;
					shieldObject.SetActive(true);
					shieldCollider.enabled = true;
				}
			}

			if (timeToEnergyGain <= 0) {
				timeToEnergyGain = baseEnergyInterval;
				energy += baseEnergyGain;
			}
		} else {
			Destroy(gameObject);
		}
	}

	void CommandDeath() {
		isAlive = false;
	}

	public GameObject RequestBuildingTarget() {
		GameObject target = null;
		int potentialTarget = 0;

		while (target == null) {
			if (tiles[potentialTarget]) {
				TileController potentialTargetTile = tiles[potentialTarget].GetComponent<TileController>();

				if (potentialTargetTile.building) {
					target = potentialTargetTile.building.gameObject;
				}
			}

			potentialTarget++;
		}

		target = gameObject;

		return target;
	}

	public PlatformController GetTargetPlatform() {
		if (!targetPlatform) {
			RetargetPlatform();
		}

		return targetPlatform;
	}

	public void SetActiveTile(TileController tile) {
		if (activeTile) {
			if (activeTile.GetInstanceID() != tile.GetInstanceID()) {
				activeTile.SetActive(false);
				activeTile = tile;
				activeTile.SetActive(true);
			}
		} else {
			activeTile = tile;
			activeTile.SetActive(true);
		}

		if (activeTile && !activeTile.building && isPlayer) {
			uiHandler.SetBuildUI(buildButtons);
		}
	}

	public void RequestBuild(string name) {
		GameObject matchingBuilding, newBuilding;
		BuildingController building;

		if (activeTile && activeTile.building == null) {
			matchingBuilding = buildingPrefabs[0];
			for (int i = 0; i < buildingPrefabs.Length; i++) {
				if (buildingPrefabs[i].name == name) {
					matchingBuilding = buildingPrefabs[i];
					break;
				}
			}

			building = matchingBuilding.GetComponent<BuildingController>();

			if (building.cost <= energy) {
				newBuilding = Instantiate(matchingBuilding, activeTile.transform.position, activeTile.transform.rotation) as GameObject;
				newBuilding.transform.parent = transform;
				activeTile.building = newBuilding.GetComponent<BuildingController>();
				activeTile.building.tile = activeTile;

				energy -= building.cost;
			}
		}
	}

	public void RequestPlatformRetarget(PlatformController platform) {
		targetPlatform = platform;
	}

	public void RetargetPlatform() {
		GameObject[] platforms;
		GameObject newTargetPlatform = null;
		int randomSelector = 0;

		platforms = GameObject.FindGameObjectsWithTag("platform");

		if (platforms.Length >= 1) {
			while(!newTargetPlatform) {
				randomSelector = Random.Range(0, platforms.Length);

				if (gameObject.GetInstanceID() != platforms[randomSelector].GetInstanceID()) {
					newTargetPlatform = platforms[randomSelector];
				}
			}

			targetPlatform = newTargetPlatform.GetComponent<PlatformController>();
		}
	}

	public float GetLeakChance(float speed) {
		float chance = 1 - (Mathf.Sin(speed / guaranteedLeakSpeed)) + Mathf.Sin(shield / shieldMax) * shieldLeakModifier;//(speed / guaranteedLeakSpeed) + ((shieldMax - shield) / shieldMax) * shieldLeakModifier;
		Debug.Log(chance);
		return chance;
	}

	public void AddShield(float amount) {
		if (shieldActive) {
			shield += amount;

			if (shield > shieldMax) {
				shield = shieldMax;
			}
		}
	}

	public void DamageShield(float amount) {
		if (shieldActive) {
			shield -= amount;
		}
	}

	public void ChangeMaxShield(float amount) {
		shieldMax += amount;
	}

	public void AddEnergy(float amount) {
		energy += amount;
	}

	public void Damage(float amount) {
		health -= amount;
	}

	public bool CanBuildTarget(string buildingName) {
		GameObject matchingBuilding = buildingPrefabs[0];
		BuildingController building;

		for (int i = 0; i < buildingPrefabs.Length; i++) {
			if (buildingPrefabs[i].name == name) {
				matchingBuilding = buildingPrefabs[i];
				break;
			}
		}
		building = matchingBuilding.GetComponent<BuildingController>();

		if (building.cost <= energy) {
			return true;
		} else {
			return false;
		}
	}
}
