using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SliderLabelHandler : MonoBehaviour {

	public Text sliderLabel;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	public void SetLabelText (float text) {
		sliderLabel.text = text.ToString();
	}
}
