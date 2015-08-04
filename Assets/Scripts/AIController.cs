using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIController : MonoBehaviour {
	public PlatformController platform;
	public string aiType = "defensive";
	public string aiDifficulty = "easy";
	public float energyModifier;
	public float aps;
	public float actionStore;
	public float maxActionStore;

	//to be moved to private
	public Dictionary<string, float> pressure = new Dictionary<string, float>();
	public float[] viewablePressures = new float[4];
	public Dictionary<string, float> pressureRates = new Dictionary<string, float>();
	public string[] pressures = {"silo", "generator", "booster"};
	public string state = "idle";

	//base AI params
	float siloPressureRate = 1;
	float generatorPressureRate = 2;
	float boosterPressureRate = 1.5f;

	//easy AI params
	float easyEnergyModifier = 1;
	float easyAPS = 0.5f;
	float easyMaxActionStore = 2;

	//normal AI params
	float normalEnergyModifier = 1.2f;
	float normalAPS = 1;
	float normalMaxActionStore = 4;

	//defensive AI params
	float defensiveSiloPressureRateModifier = 0.8f;
	float defensiveGeneratorPressureRateModifier = 1;
	float defensiveBoosterPressureRateModifier = 1.5f;

	// Use this for initialization
	void Start () {
		switch (aiDifficulty) {
			case "easy":
				energyModifier = easyEnergyModifier;
				aps = easyAPS;
				maxActionStore = easyMaxActionStore;
				break;

			case "normal":
			default:
				energyModifier = normalEnergyModifier;
				aps = normalAPS;
				maxActionStore = normalMaxActionStore;
				break;
		}

		switch (aiType) {
			case "defensive":
			default:
				pressureRates.Add("silo", siloPressureRate * defensiveSiloPressureRateModifier);
				pressureRates.Add("booster", boosterPressureRate * defensiveBoosterPressureRateModifier);
				pressureRates.Add("generator", generatorPressureRate * defensiveGeneratorPressureRateModifier);
				break;
		}

		for (int i = 0; i < pressures.Length; i++) {
			pressure.Add(pressures[i], 0);
		}
	}

	// Update is called once per frame
	void Update () {
		TileController newActiveTile;
		string highestPressure = "silo";
		string desiredBuild = "";

		int i = 0;
		foreach (var pair in pressureRates) {
			pressure[pair.Key] += pair.Value * Time.deltaTime;

			if (pressure[pair.Key] > pressure[highestPressure]) {
				highestPressure = pair.Key;
			}

			viewablePressures[i] = pressure[pair.Key];
			i++;
		}

		actionStore += aps * Time.deltaTime;
		if (actionStore > maxActionStore) {
			actionStore = maxActionStore;
		}

		if (state != "defeated" && platform) {
			if (actionStore >= 1) {
				switch (highestPressure) {
					case "silo":
					desiredBuild = "Rocket Silo";
					break;

					case "generator":
					desiredBuild = "Solar Generator";
					break;

					case "booster":
					desiredBuild = "Small Booster";
					break;

					default:
					break;
				}

				if (CanBuild(desiredBuild)) {
					newActiveTile = FindEmptyTile();

					if (newActiveTile) {
						platform.SetActiveTile(newActiveTile);
						platform.RequestBuild(desiredBuild);
						actionStore--;
						pressure[highestPressure] = 0;
					}
				}
			}
		} else {
			state = "defeated";
		}
	}

	void SelectTile(TileController tile) {
		platform.SetActiveTile(tile);
		actionStore--;
	}

	void BuildBuilding(string name) {
		platform.RequestBuild(name);
		actionStore--;
	}

	void InitializeAI(PlatformController ownedPlatform) {
		platform = ownedPlatform;
	}

	TileController FindEmptyTile() {
		for (int i = 0; i < platform.tileControllers.Length; i++) {
			if (platform.tileControllers[i] && !platform.tileControllers[i].building) {
				return platform.tileControllers[i];
			}
		}

		return null;
	}

	string FindHighestPressure() {
		float highestPressure = 0;
		string highestPressureName = "";

		foreach (var pair in pressure) {
			if (pair.Value > highestPressure) {
				highestPressureName = pair.Key;
			}
		}

		return highestPressureName;
	}

	bool CanBuild(string buildingName) {
		if (platform.CanBuildTarget(buildingName)) {
			return true;
		} else {
			return false;
		}
	}
}
