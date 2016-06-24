using UnityEngine;
using System.Collections;
using System.IO;
using System;
using ZenFulcrum.EmbeddedBrowser;

public class ConfigHandler : MonoBehaviour {

	public GameObject browserContainer;
	public Texture2D[] platformTextures;
	public string[] platformNames;
	public int[][] platformIdentities;
	public int numEnemies = 1;
	public int difficulty = 1;

	Browser browser;

	// Use this for initialization
	void Start () {
		string thisDirectory = Directory.GetCurrentDirectory();
		bool platformDirectory = false;

		browser = browserContainer.GetComponent<Browser>();

		DirectoryInfo dir = new DirectoryInfo(thisDirectory + @"\Platforms");

		try {
			if (dir.Exists) {
				platformDirectory = true;
			} else {
				dir.Create();
				platformDirectory = true;
			}
		} catch (Exception e) {
			platformDirectory = false;
			Debug.Log(e);
		}

		if (platformDirectory) {
			FileInfo[] info = dir.GetFiles("*.png");
			platformTextures = new Texture2D[info.Length];
			platformNames = new string[info.Length];
			platformIdentities = new int[info.Length][];

			int i = 0;
			foreach (FileInfo f in info) {
				WWW currentTexture = new WWW("file:///" + f);

				platformTextures[i] = currentTexture.texture;
				platformNames[i] = Path.GetFileNameWithoutExtension(f + "");
				platformIdentities[i] = TextureToIdentity(platformTextures[i]);
				i++;
			}

			browser.CallFunction("Messaging.trigger", "platforms", JsonUtility.ToJson(new string[1]{"test"}));
			Debug.Log(JsonUtility.ToJson(new string[1]{"test"}));
		}
	}

	// Update is called once per frame
	void Update () {

	}

	int[] TextureToIdentity(Texture2D texture) {
		Color32[] pixels = texture.GetPixels32();
		int[] identity = new int[pixels.Length];
		Color32 emptyColor = Color.white;
		string emptyColorString = emptyColor.ToString();

		for (int i = 0; i < pixels.Length; i++) {
			identity[i] = pixels[i].ToString() == emptyColorString ? 0 : 1;
		}

		return identity;
	}

	public void SetNumEnemies(float num) {
		numEnemies = (int)num;
	}
}
