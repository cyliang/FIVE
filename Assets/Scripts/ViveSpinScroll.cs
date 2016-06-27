using UnityEngine;
using System.Collections;

public class ViveSpinScroll : MonoBehaviour {

    public delegate void SpinScrollEventHandler(int scrollAmount);
    public event SpinScrollEventHandler SpinScrolled;

    public float degreePerEvent;
	public static bool enable = true;

    private SteamVR_TrackedController controller;
    private bool previousTouched;
    private float previousAngle;
    private float accumulatedSpin;

	void Start () {
        controller = GetComponent<SteamVR_TrackedController>();
        previousTouched = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (!controller.padTouched || !enable) {
            previousTouched = false;
            return;
        }

        float nowAngle = calculateAngle(controller.controllerState.rAxis0);
        if (!previousTouched) {
            previousTouched = true;
            accumulatedSpin = 0;

        } else {
            accumulatedSpin += Mathf.DeltaAngle(previousAngle, nowAngle);
            int scrollAmount = (int)(accumulatedSpin / degreePerEvent);
            if (Mathf.Abs(scrollAmount) >= 1) {
                accumulatedSpin -= scrollAmount * degreePerEvent;
                SpinScrolled(-scrollAmount);
            }
        }
        previousAngle = nowAngle;
    }

    float calculateAngle(Valve.VR.VRControllerAxis_t coor) {
        return Mathf.Atan2(coor.y, coor.x) * Mathf.Rad2Deg;
    }
}
