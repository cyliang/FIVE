using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class InputController: MonoBehaviour {
	public SteamVR_LaserPointer _laserPointer;
    public SteamVR_TrackedController controller;

	public static SteamVR_Controller.Device rightController {
		get {
			return SteamVR_Controller.Input (SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost));
		}
	}
	public static SteamVR_LaserPointer laserPointer {
		get {
			return instance._laserPointer;
		}
	}

	private static InputController instance;

    private Transform laserOn, clickOn;

	void Start() {
		instance = this;

		laserPointer.PointerIn += (object sender, PointerEventArgs e) => {
            laserOn = e.target;
            
            Selectable s = e.target.GetComponent<Selectable>();
            if (s) {
                PointerEventData data = new PointerEventData(EventSystem.current);
                data.pointerPress = null;
                s.OnPointerEnter(data);
            }
		};
		laserPointer.PointerOut += (object sender, PointerEventArgs e) => {
            laserOn = null;

            Selectable s = e.target.GetComponent<Selectable>();
            if (s) {
                PointerEventData data = new PointerEventData(EventSystem.current);
                data.pointerPress = null;
                s.OnPointerExit(data);
            }
        };
        controller.TriggerClicked += (object sender, ClickedEventArgs e) => {
            if (laserOn) {
                clickOn = laserOn;

                Selectable s = laserOn.GetComponent<Selectable>();
                if (s) {
                    PointerEventData data = new PointerEventData(EventSystem.current);
                    data.pointerPress = laserOn.gameObject;
                    data.button = PointerEventData.InputButton.Left;
                    s.OnPointerDown(data);
                }
            }
        };
        controller.TriggerUnclicked += (object sender, ClickedEventArgs e) => {
            PointerEventData data = new PointerEventData(EventSystem.current);
            data.pointerPress = laserOn != null ? laserOn.gameObject : null;
            data.button = PointerEventData.InputButton.Left;

            if (laserOn != null) {
                Selectable s = laserOn.GetComponent<Selectable>();
                if (s)
                    s.OnPointerUp(data);
            }
            if (clickOn != null) {
                Selectable s = clickOn.GetComponent<Selectable>();
                if (s)
                    s.OnPointerUp(data);
            }

            if (clickOn == laserOn) {
                Button s = clickOn.GetComponent<Button>();
                if (s)
                    s.OnPointerClick(data);
            }
        };

        //data.button = PointerEventData.InputButton.Left;
    }
}
