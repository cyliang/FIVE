using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MenuController : MonoBehaviour {

	public static bool isActive {
		get {
			return instance.canvas.activeSelf;
		}
	}
	public GameObject camera;
	public GameObject canvas;
	public GameObject menuBtnPrefab;

	private static MenuController instance;

	private Dictionary<GameObject, Button> buttons = new Dictionary<GameObject, Button>();

	// Use this for initialization
	void Start () {
		instance = this;

		GameObject createViewBtn = Instantiate (menuBtnPrefab);
		createViewBtn.transform.SetParent(canvas.transform, false);
		buttons.Add (createViewBtn, createViewBtn.GetComponent<Button>());
	}
	
	// Update is called once per frame
	void Update () {
		if (InputController.rightController.GetPressDown (SteamVR_Controller.ButtonMask.ApplicationMenu)) {
			if (isActive) {
				canvas.SetActive (false);
			} else {
				canvas.SetActive (true);
				transform.eulerAngles = new Vector3(0f, camera.transform.eulerAngles.y, 0f);
			}
		}
	}
}
