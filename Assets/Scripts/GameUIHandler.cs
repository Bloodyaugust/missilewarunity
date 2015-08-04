using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameUIHandler : MonoBehaviour {

	public GameObject[] buttonPrefabs;
	public GameObject uiPanel;
	public Text energyText;
	public Text shieldText;
	public PlatformController playerPlatform;

	ButtonHandler[] buttonHandlers;
	GameObject[] buttons;
	GameObject[] activeButtons;

	// Use this for initialization
	void Start () {
		buttons = new GameObject[buttonPrefabs.Length];
		buttonHandlers = new ButtonHandler[buttonPrefabs.Length];
		activeButtons = new GameObject[buttonPrefabs.Length];

		for (int i = 0; i < buttonPrefabs.Length; i++) {
			buttons[i] = Instantiate(buttonPrefabs[i], Vector3.zero, Quaternion.identity) as GameObject;
			buttonHandlers[i] = buttons[i].GetComponent<ButtonHandler>();

			buttons[i].transform.SetParent(uiPanel.transform, false);
			AddBuildRequestListener(buttons[i].GetComponent<Button>(), buttonHandlers[i].fullType);
			buttons[i].SetActive(false);
		}
	}

	// Update is called once per frame
	void Update () {
		energyText.text = "Energy\n" + playerPlatform.energy;
		shieldText.text = "Shield\n" + (playerPlatform.shield / playerPlatform.shieldMax) * 100 + "%";
	}

	void AddBuildRequestListener(Button b, string buildingName) {
		b.onClick.AddListener(() => SendBuildRequest(buildingName));
	}

	public void SetBuildUI(string[] possibleBuildButtons) {
		for (int i = 0; i < activeButtons.Length; i++) {
			if (activeButtons[i]) {
				activeButtons[i].gameObject.SetActive(false);
				activeButtons[i] = null;
			}
		}

		for (int i = 0; i < possibleBuildButtons.Length; i++) {
			for (int i2 = 0; i2 < buttons.Length; i2++) {
				if (buttonHandlers[i2].fullType == possibleBuildButtons[i]) {
					buttons[i2].SetActive(true);
					activeButtons[i2] = buttons[i2];
					break;
				}
			}
		}
	}

	public void SendBuildRequest(string name) {
		playerPlatform.RequestBuild(name);
	}
}
