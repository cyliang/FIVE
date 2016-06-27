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

    private bool _menuActive;
	public static bool isActive {
		get {
			return instance._menuActive;
		}
        set {
            if (value && !instance._menuActive) {
                createOptionMenu(instance.menuBtns, new Vector3(0f, instance.mainCam.transform.eulerAngles.y, 0f));
            } else if (!value && instance._menuActive) {
                clearOptionMenu();
            }
            instance._menuActive = value;
        }
	}
	private Camera mainCam;
	public GameObject optionCanvas;
	public GameObject optionParent;
    public GameObject optionBtnPrefab;
    public Image optionHalo;
	public SteamVR_TrackedController controller;

	private static MenuController instance;

	private List<Option> menuBtns = new List<Option>();

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
        instance.menuBtns.Add(new Option {
            text = txt,
            onClick = onClick
        });
    }

    public static void createOptionMenu(IList<Option> options, Vector3 angle) {
        clearOptionMenu();
		instance._menuActive = false;

        instance.optionRadian = Mathf.PI * 2f / options.Count;
		instance.optionDistance = options.Count < 5 ? 1f : (0.5f / Mathf.Tan(instance.optionRadian));

        float accuAngle = 0f;
        foreach (Option option in options) {
            Image newBtn = Instantiate(instance.optionBtnPrefab).GetComponent<Image>();
			Button btn = newBtn.GetComponent<Button> ();
            newBtn.GetComponentInChildren<Text>().text = option.text;
			newBtn.transform.SetParent (instance.optionParent.transform, false);

            newBtn.rectTransform.localPosition = new Vector3(Mathf.Cos(accuAngle), Mathf.Sin(accuAngle)) * instance.optionDistance;
            accuAngle += instance.optionRadian;

            instance.optionBtns.Add(new OptionBtn {
                option = option,
                image = newBtn
			});
			btn.onClick.AddListener (clearOptionMenu);
			btn.onClick.AddListener (option.onClick);
        }

        instance.transform.eulerAngles = angle;
        instance.optionCanvas.SetActive(true);
		ViveSpinScroll.enable = false;
    }

    public static void clearOptionMenu() {
        foreach (var btn in instance.optionBtns) {
            Destroy(btn.image.gameObject);
        }
        instance.optionBtns.Clear();
        instance.optionCanvas.SetActive(false);
		instance._menuActive = false;
        instance.optionSelected = null;
		ViveSpinScroll.enable = true;
    }

    void updateOptionMenu() {
        Vector2 haloPos = new Vector2(controller.controllerState.rAxis0.x, controller.controllerState.rAxis0.y);
        optionHalo.rectTransform.localPosition = haloPos * optionDistance;
		var newSelected = optionBtns[Mathf.FloorToInt(((Mathf.Atan2(haloPos.y, haloPos.x) + optionRadian / 2 + 2 * Mathf.PI) % (2 * Mathf.PI)) / optionRadian)];

		if (optionSelected != null && newSelected != optionSelected) {
			optionSelected.image.color = Color.white;
		}

		optionSelected = newSelected;
		optionSelected.image.color = Color.green;
    }

    void onOptionClicked(object sender, ClickedEventArgs e) {
        var onClick = optionSelected.option.onClick;
        clearOptionMenu();
        onClick();
    }

}
