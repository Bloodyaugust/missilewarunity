using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public PlatformController[] platforms;
	public GameUIHandler uiHandler;
	public GameObject platformPrefab;
	public GameObject aiControllerPrefab;
	public float platformDistance;

	ConfigHandler cfgHandler;

	// Use this for initialization
	void Start () {

	}

	void Awake () {
		PlatformController currentPlatformController;
		GameObject configObject = GameObject.FindWithTag("config");
		GameObject currentPlatform, currentAIController;
		Vector3 currentPlatformPosition;
		float currentAngle = 0;
		int[] cells = new int[25];
		cells[6] = 1;
		cells[7] = 1;
		cells[8] = 1;
		cells[10] = 1;
		cells[11] = 1;
		cells[12] = 1;
		cells[13] = 1;
		cells[14] = 1;
		cells[16] = 1;
		cells[17] = 1;
		cells[18] = 1;
		int numEnemies = 1;
		int difficulty = 1;
		int numPlatforms = 1;

		if (configObject) {
			cfgHandler = GameObject.FindWithTag("config").GetComponent<ConfigHandler>();
		}

		if (cfgHandler) {
			Debug.Log(cfgHandler.numEnemies);
			if (cfgHandler.numEnemies > 0) {
				numEnemies = cfgHandler.numEnemies;
			}
			if (cfgHandler.difficulty > 0) {
				difficulty = cfgHandler.difficulty;
			}
		}

		numPlatforms += numEnemies;
		platforms = new PlatformController[numPlatforms];

		for (int i = 0; i < numPlatforms; i++) {
			currentPlatformPosition = new Vector3(platformDistance * Mathf.Cos(currentAngle), platformDistance * Mathf.Sin(currentAngle), 0);

			currentPlatform = Instantiate(platformPrefab, currentPlatformPosition, Quaternion.identity) as GameObject;
			currentPlatformController = currentPlatform.GetComponent<PlatformController>();
			currentAngle += (2 * Mathf.PI) / numPlatforms;
			platforms[i] = currentPlatformController;

			currentPlatformController.team = i;
			currentPlatformController.owner = i;
			currentPlatformController.isPlayer = (i == 0) ? true : false;

			if (i == 0) {
				uiHandler.playerPlatform = currentPlatformController;
				currentPlatformController.uiHandler = uiHandler;
			} else {
				currentAIController = Instantiate(aiControllerPrefab, currentPlatformPosition, Quaternion.identity) as GameObject;
				currentAIController.SendMessage("InitializeAI", currentPlatformController);
			}

			currentPlatformController.SendMessage("Construct", cells);
		}

		for (int i = 0; i < numPlatforms; i++) {
			platforms[i].GetComponent<PlatformController>().RetargetPlatform();
		}
	}

	// Update is called once per frame
	void Update () {

	}
}
