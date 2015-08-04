using UnityEngine;
using System.Collections;

public class CameraHandler : MonoBehaviour {

	float speed = 500;

	// Use this for initialization
	void Start () {
		GetComponent<Camera>().orthographicSize = Screen.height / 2f;
	}

	// Update is called once per frame
	void Update () {
		transform.Translate(Input.GetAxis("Horizontal") * Time.deltaTime * speed, Input.GetAxis("Vertical") * Time.deltaTime * speed, 0);
	}
}
