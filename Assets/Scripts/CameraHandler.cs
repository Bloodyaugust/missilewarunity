using UnityEngine;
using System.Collections;

public class CameraHandler : MonoBehaviour {

	Camera worldCam;
	float speed = 500;
	float zoomSpeed = 2000;
	float minZoom = 100;
	float maxZoom = 500;

	// Use this for initialization
	void Start () {
		worldCam = GetComponent<Camera>();
		worldCam.orthographicSize = Screen.height / 2f;
	}

	// Update is called once per frame
	void Update () {
		float zoomChange = Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomSpeed;

		transform.Translate(Input.GetAxis("Horizontal") * Time.deltaTime * speed, Input.GetAxis("Vertical") * Time.deltaTime * speed, 0);

		if (worldCam.orthographicSize + zoomChange >= minZoom && worldCam.orthographicSize + zoomChange <= maxZoom) {
			worldCam.orthographicSize += zoomChange;
		}

		if (Input.GetAxis("ResetCamera") > 0) {
			worldCam.orthographicSize = Screen.height / 2f;
		}
	}
}
