using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuController : MonoBehaviour {

	public static bool isActive {
		get {
			return instance.canvas.activeSelf;
		}
	}
	public GameObject camera;
	public GameObject canvas;
	public GameObject menuBtn;

	private static MenuController instance;

	private Button createViewBtn;

	// Use this for initialization
	void Start () {
		instance = this;
		createViewBtn = Instantiate (menuBtn).GetComponent<Button>();
		createViewBtn.transform.SetParent(canvas.transform, false);

		InputController.laserPointer.PointerIn += onLaserIn;
		InputController.laserPointer.PointerOut += onLaserOut;
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

	void onLaserIn(object sender, PointerEventArgs e) {
	}

	void onLaserOut(object sender, PointerEventArgs e) {
	}
}
