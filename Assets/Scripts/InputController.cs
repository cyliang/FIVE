using UnityEngine;
using System.Collections;

public class InputController {
	public static SteamVR_Controller.Device rightController {
		get {
			return SteamVR_Controller.Input (SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost));
		}
	}
}
