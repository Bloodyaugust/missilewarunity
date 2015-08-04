using UnityEngine;
using System.Collections;

public class ConfigHandler : MonoBehaviour {

	public int numEnemies = 1;
	public int difficulty = 1;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(transform.gameObject);
	}

	// Update is called once per frame
	void Update () {

	}

	public void SetNumEnemies(float num) {
		numEnemies = (int)num;
	}

	public void RemoveConfig() {
		Destroy(transform.gameObject);
	}
}
