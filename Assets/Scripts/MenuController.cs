using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class MenuController : MonoBehaviour {

    public class Option {
        public string text;
        public UnityEngine.Events.UnityAction onClick;
    }

	public static bool isActive {
		get {
			return instance.canvas.activeSelf;
		}
        set {
            instance.canvas.SetActive(value);
            if (value) {
                instance.transform.eulerAngles = new Vector3(0f, instance.mainCam.transform.eulerAngles.y, 0f);
                clearOptionMenu();
            }
        }
	}
	private Camera mainCam;
	public GameObject canvas, panel;
	public GameObject menuBtnPrefab;
    public GameObject optionCanvas;
    public GameObject optionBtnPrefab;
    public Image optionHalo;
	public SteamVR_TrackedController controller;

	private static MenuController instance;

	private Dictionary<GameObject, Button> buttons = new Dictionary<GameObject, Button>();

    private class OptionBtn {
        public Option option;
        public Image image;
    }
    private List<OptionBtn> optionBtns = new List<OptionBtn>();
    private OptionBtn optionSelected = null;
    private float optionDistance, optionRadian;

	// Use this for initialization
	void Start () {
		instance = this;
        mainCam = Camera.main;

		controller.MenuButtonClicked += (sender, e) => {
            isActive = !isActive;
		};
        controller.PadTouched += (sender, e) => {
            optionHalo.gameObject.SetActive(true);
        };
        controller.PadUntouched += (sender, e) => {
            optionHalo.gameObject.SetActive(false);
            if (optionSelected != null) {
                optionSelected.image.color = Color.white;
                optionSelected = null;
            }
        };
        controller.PadClicked += (sender, e) => {
            if (optionSelected != null)
                optionSelected.image.color = Color.red;
        };
        controller.PadUnclicked += onOptionClicked;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.F4))
            isActive = !isActive;

        if (controller.padTouched && optionCanvas.activeSelf)
            updateOptionMenu();
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

    public static void createOptionMenu(IList<Option> options, Vector3 angle) {
        clearOptionMenu();
        isActive = false;

        instance.optionRadian = Mathf.PI * 2f / options.Count;
        instance.optionDistance = 0.5f / Mathf.Tan(instance.optionRadian);

        float accuAngle = 0f;
        foreach (Option option in options) {
            Image newBtn = Instantiate(instance.optionBtnPrefab).GetComponent<Image>();
            newBtn.GetComponentInChildren<Text>().text = option.text;

            newBtn.rectTransform.position = new Vector3(Mathf.Cos(accuAngle), Mathf.Sin(accuAngle)) * instance.optionDistance;
            accuAngle += instance.optionRadian;

            instance.optionBtns.Add(new OptionBtn {
                option = option,
                image = newBtn
            });
        }

        instance.transform.eulerAngles = angle;
        instance.optionCanvas.SetActive(true);
    }

    public static void clearOptionMenu() {
        foreach (var btn in instance.optionBtns) {
            Destroy(btn.image.gameObject);
        }
        instance.optionBtns.Clear();
        instance.optionCanvas.SetActive(false);
        instance.optionSelected = null;
    }

    void updateOptionMenu() {
        Vector2 haloPos = new Vector2(controller.controllerState.rAxis0.x, controller.controllerState.rAxis0.y);
        optionHalo.rectTransform.position = haloPos * optionDistance;
        optionSelected = optionBtns[Mathf.FloorToInt(Mathf.Atan2(haloPos.y, haloPos.x) / optionRadian)];

        optionSelected.image.color = Color.green;
    }

    void onOptionClicked(object sender, ClickedEventArgs e) {
        var onClick = optionSelected.option.onClick;
        clearOptionMenu();
        onClick();
    }

}
