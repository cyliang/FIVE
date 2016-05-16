using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ViewController : MonoBehaviour {

    public GameObject viewPrefab;
    public GameObject UIObject;
	public SteamVR_LaserPointer laserPointer;

    private Vector3 viewOrigScale;
    private LinkedList<ViewBehavior> displayedViewList = new LinkedList<ViewBehavior>();
    private LinkedList<ViewBehavior> hiddenViewList = new LinkedList<ViewBehavior>();
    private GameObject viewsObject;

	private Transform pointerOn = null;
	private Transform draggingView = null;
	private SteamVR_Controller.Device input {
		get {
			return SteamVR_Controller.Input (SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost));
		}
	}
	private float gripTime = 0;

    private bool _isInUI;
    private bool isInUI {
        get { return _isInUI; }
        set {
            _isInUI = value;
            if (value)
                showViewUI();
            else
                rearrange();
        }
    }

    private bool _isEditing;
    private bool isEditing {
        get { return _isEditing; }
        set {
            foreach (ViewBehavior view in displayedViewList.Concat(hiddenViewList)) {
                view.isShaking = value;
            }
            _isEditing = value;
        }
    }

    // Use this for initialization
    void Start () {
        viewsObject = new GameObject("Views");
        viewsObject.transform.parent = transform;
        
        viewOrigScale = viewPrefab.transform.localScale;
        isInUI = false;

		laserPointer.PointerIn += OnLaserPointerIn;
		laserPointer.PointerOut += OnLaserPointerOut;
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.F1)) {
            createView();
        } else if (Input.GetKeyDown(KeyCode.F2)) {
            isInUI = !isInUI;
        } else if (Input.GetKeyDown(KeyCode.F3)) {
            if (isInUI) {
                isEditing = !isEditing;
            }
        }

		checkPressedDown ();
	}

    void createView() {
        ViewBehavior newView = Instantiate(viewPrefab).GetComponent<ViewBehavior>();
        newView.transform.parent = viewsObject.transform;
        newView.viewController = this;
        newView.selfNode = displayedViewList.Count < 6 ? displayedViewList.AddLast(newView) : hiddenViewList.AddLast(newView);
        rearrange();
    }

    public void removeView(LinkedListNode<ViewBehavior> view) {
        Destroy(view.Value.gameObject);
        view.List.Remove(view);
        showViewUI();
    }

    void rearrange() {
        UIObject.SetActive(false);
        isEditing = false;
        _isInUI = false;

        showSurroundedViews(displayedViewList, viewOrigScale, 360 / 6);
        foreach (ViewBehavior view in hiddenViewList) {
            view.gameObject.SetActive(false);
        }
    }

    void showViewUI() {
        UIObject.SetActive(true);
        _isInUI = true;

        Vector3 previewScale = new Vector3(0.28f, 0.28f, 1f);
        showSurroundedViews(displayedViewList, previewScale, 120 / 6, -22f);
        showSurroundedViews(hiddenViewList, previewScale, 120 / 6, 27f);
    }

    void showSurroundedViews(IEnumerable<ViewBehavior> showList, Vector3 scale, float deltaAngle, float elevationAngle = 0f) {
        float firstAngle = -(showList.Count() - 1) / 2f * deltaAngle;
        foreach (ViewBehavior view in showList) {
            view.gameObject.SetActive(true);
            view.transform.localScale = scale;
            view.transform.eulerAngles = new Vector3(elevationAngle, firstAngle, 0);
            firstAngle += deltaAngle;
        }
    }

	void OnLaserPointerIn(object sender, PointerEventArgs e) {		
		if (pointerOn == null) {
			pointerOn = e.target;
		}
	}

	void OnLaserPointerOut(object sender, PointerEventArgs e) {
		pointerOn = null;
	}

	void checkPressedDown() {
		var _input = input;

		if (_input.GetPressDown (SteamVR_Controller.ButtonMask.ApplicationMenu)) {
			createView();
		}

		if (_input.GetPressDown (SteamVR_Controller.ButtonMask.Grip)) {
			gripTime = Time.time;
		} else if (_input.GetPress (SteamVR_Controller.ButtonMask.Grip) && gripTime != -1f && Time.time - gripTime >= 0.5f && isInUI) {
			isEditing = !isEditing;
			gripTime = -1f;
		} else if (_input.GetPressUp (SteamVR_Controller.ButtonMask.Grip) && gripTime != -1f) {
			isInUI = !isInUI;
		}

		if (!isEditing || pointerOn == null)
			return;
		
		if (_input.GetPressDown (SteamVR_Controller.ButtonMask.Trigger)) {
			if (pointerOn.CompareTag("ViewPlane") && displayedViewList.Concat (hiddenViewList).Contains (pointerOn.parent.gameObject.GetComponent<ViewBehavior> ()))
				draggingView = pointerOn.parent;
			
			if (pointerOn.CompareTag ("ViewClose")) {
				ViewBehavior view = pointerOn.parent.parent.gameObject.GetComponent<ViewBehavior> ();
				if (displayedViewList.Concat (hiddenViewList).Contains (view))
					view.OnCloseBtnPressed ();
			}
		}

		if (_input.GetPressUp (SteamVR_Controller.ButtonMask.Trigger)) {
			draggingView = null;
		}

		if (draggingView != null) {
			Vector3 targetPosition = laserPointer.transform.position + laserPointer.transform.forward.normalized * laserPointer.pointerDistance;
			draggingView.rotation = Quaternion.FromToRotation (Vector3.forward, targetPosition);
		}
		//SteamVR_Controller.Input(deviceIndex).TriggerHapticPulse(1000);
	}
}
