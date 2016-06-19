using UnityEngine;
using System.Collections;

public class SkyChanger : MonoBehaviour {

	private int currentBox;

	// Use this for initialization
	void Start () {
		RenderSettings.skybox = (Material)Resources.Load("Skybox3");
		currentBox = 2;
	}
	
	// Update is called once per frame
	void Update () {
		int nextBox = (int) (Time.time / 5) % 6;
		if (nextBox != currentBox) {
			currentBox = nextBox;
			RenderSettings.skybox = (Material)Resources.Load("Skybox" + (nextBox + 1).ToString ());
		}
	}
}
