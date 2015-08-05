using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class ConfigHandler : MonoBehaviour {

	public Texture2D[] platformTextures;
	public int[][] platformIdentities;
	public int numEnemies = 1;
	public int difficulty = 1;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(transform.gameObject);
		string thisDirectory = Directory.GetCurrentDirectory();
		bool platformDirectory = false;

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
			platformIdentities = new int[info.Length][];

			int i = 0;
			foreach (FileInfo f in info) {
				WWW currentTexture = new WWW("file:///" + f);

				platformTextures[i] = currentTexture.texture;
				platformIdentities[i] = TextureToIdentity(platformTextures[i]);
				i++;
			}

			Debug.Log(platformIdentities);
		}

		GameObject camera = GameObject.FindWithTag("MainCamera");
		camera.SendMessage("Init");
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

	public void RemoveConfig() {
		Destroy(transform.gameObject);
	}
}
