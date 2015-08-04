using UnityEngine;
using System.Collections;

public class TileController : MonoBehaviour {

	public BuildingController building;
	public Sprite[] tileSprites;
	public Color activeTint;

	public SpriteRenderer spriteRenderer;
	public PlatformController parentPlatform;

	void Awake () {
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	// Use this for initialization
	void Start () {
		parentPlatform = transform.parent.GetComponent<PlatformController>();

		spriteRenderer.sprite = tileSprites[(int)Mathf.Floor(Random.Range(0f, 2.99f))];
	}

	// Update is called once per frame
	void Update () {

	}

	void OnMouseUp () {
		if (parentPlatform.isPlayer) {
			parentPlatform.SetActiveTile(this);
		}
	}

	public void SetActive (bool active) {
		spriteRenderer.color = active ? new Color(255, 255, 0, 255) : Color.white;
	}
}
