using UnityEngine;
using System.Collections;

public class MenuBehavior : MonoBehaviour {

	public static bool isActive {
		get {
			return instance.gameObject.activeSelf;
		}
		set {
			instance.gameObject.SetActive (true);
		}
	}

	private static MenuBehavior instance;

	// Use this for initialization
	void Start () {
		instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		if (InputController.rightController.GetPressDown (SteamVR_Controller.ButtonMask.ApplicationMenu)) {
			ViewController.instance.createView ();
		}
	}
}
