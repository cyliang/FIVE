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
	}
	public GameObject camera;
	public GameObject canvas;
	public GameObject menuBtnPrefab;
	public SteamVR_TrackedController controller;

	private static MenuController instance;

	private Dictionary<GameObject, Button> buttons = new Dictionary<GameObject, Button>();

	// Use this for initialization
	void Start () {
		instance = this;

		controller.MenuButtonClicked += (object sender, ClickedEventArgs e) => {
			if (isActive) {
				canvas.SetActive (false);
			} else {
				canvas.SetActive (true);
				transform.eulerAngles = new Vector3(0f, camera.transform.eulerAngles.y, 0f);
			}
		};

	}
	
	// Update is called once per frame
	void Update () {
	}

    public static void addBtn(string text, UnityEngine.Events.UnityAction onClick) {
        Button btn = Instantiate(instance.menuBtnPrefab).GetComponent<Button>();
        btn.GetComponentInChildren<Text>().text = text;
        btn.transform.SetParent(instance.canvas.transform, false);
        btn.onClick.AddListener(onClick);
        instance.buttons.Add(btn.gameObject, btn);
    }
}
