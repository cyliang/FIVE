using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class MenuController : MonoBehaviour {

	public static bool isActive {
		get {
			return instance.canvas.activeSelf;
		}
        set {
            instance.canvas.SetActive(value);
            if (value)
                instance.transform.eulerAngles = new Vector3(0f, instance.mainCam.transform.eulerAngles.y, 0f);
        }
	}
	private Camera mainCam;
	public GameObject canvas, panel;
	public GameObject menuBtnPrefab;
	public SteamVR_TrackedController controller;

	private static MenuController instance;

	private Dictionary<GameObject, Button> buttons = new Dictionary<GameObject, Button>();

	// Use this for initialization
	void Start () {
		instance = this;
        mainCam = Camera.main;

		controller.MenuButtonClicked += (object sender, ClickedEventArgs e) => {
            isActive = !isActive;
		};

	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.F4))
            isActive = !isActive;
            
	}

    public static void addBtn(string txt, UnityEngine.Events.UnityAction onClick) {
		GameObject obj = Instantiate (instance.menuBtnPrefab);
        Button btn = obj.GetComponentInChildren<Button>();
        Text text = btn.GetComponentInChildren<Text>();

		text.text = txt;
		text.resizeTextForBestFit = true;
        obj.transform.SetParent(instance.panel.transform, false);
        btn.onClick.AddListener(onClick);
		btn.onClick.AddListener (() => {instance.canvas.SetActive (false);});
        instance.buttons.Add(obj, btn);
    }
}
