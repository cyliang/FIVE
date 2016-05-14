using UnityEngine;
using System.Collections;

public class ViewBehavior : MonoBehaviour {

    public GameObject webPlane;
    public float shakeSpeed, shakeAmplitude;
    public bool isShaking { get; set; }

	// Use this for initialization
	void Start () {
        isShaking = false;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 eulerAngle = transform.localEulerAngles;
        eulerAngle.z = isShaking ? Mathf.Sin(Time.time * shakeSpeed) * shakeAmplitude : 0;
        transform.localEulerAngles = eulerAngle;
	}
}
