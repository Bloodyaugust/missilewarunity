using UnityEngine;
using System.Collections;
using ZenFulcrum.EmbeddedBrowser;

public class SkirmishConfigController : MonoBehaviour {

	public GameObject browserContainer;

	Browser browser;

	// Use this for initialization
	void Start () {
		browser = browserContainer.GetComponent<Browser>();

		browser.Resize(Screen.width, Screen.height);

		var height = Camera.main.orthographicSize * 2.0f;
		var width = height * Screen.width / Screen.height;
		transform.localScale = new Vector3(width, height, 0.1f);
	}

	// Update is called once per frame
	void Update () {

	}
}
