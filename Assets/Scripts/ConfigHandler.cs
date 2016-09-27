using UnityEngine;
using System.Collections;
using System.IO;
using System;
using SimpleJSON;
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
		JSONObject configJSON = new JSONObject();
		JSONObject j = new JSONObject();
		JSONObject platforms = new JSONObject(JSONObject.Type.ARRAY);
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

			configJSON.AddField("player", "Bloodyaugust");
			configJSON.AddField("platforms", platforms);

			int i = 0;
			foreach (FileInfo f in info) {
				j = new JSONObject();
				WWW currentTexture = new WWW("file:///" + f);

				platformTextures[i] = currentTexture.texture;
				platformNames[i] = Path.GetFileNameWithoutExtension(f + "");
				platformIdentities[i] = TextureToIdentity(platformTextures[i]);

				platforms.Add(j);
				j.AddField("name", platformNames[i]);
				j.AddField("value", i.ToString());

				Debug.Log(Path.GetFileNameWithoutExtension(f + ""));

				i++;
			}

			browser.CallFunction("Messaging.trigger", "platforms", configJSON.ToString());
			Debug.Log(configJSON.ToString());
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
