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
	}

	// Update is called once per frame
	void Update () {

	}
}
