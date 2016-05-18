using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuBehavior : MonoBehaviour {

	public static bool isActive {
		get {
			return instance.canvas.activeSelf;
		}
	}
	public GameObject camera;
	public GameObject canvas;
	public GameObject menuBtn;

	private static MenuBehavior instance;

	private Button createViewBtn;

	// Use this for initialization
	void Start () {
		instance = this;
		createViewBtn = Instantiate (menuBtn).GetComponent<Button>();
		createViewBtn.transform.SetParent(canvas.transform, false);
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
