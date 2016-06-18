using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ViewBehavior : MonoBehaviour {
    
    public UWKWebView webView;
    public GameObject closeBtn;
    public float shakeSpeed, shakeAmplitude;

    public bool isShaking { get; set; }
    public ViewController viewController { get; set; }
    public LinkedListNode<ViewBehavior> selfNode { get; set; }
	public readonly FileManager.File fileOpened = new FileManager.File();

	// Use this for initialization
	void Start () {
        isShaking = false;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 eulerAngle = transform.localEulerAngles;
        eulerAngle.z = isShaking ? Mathf.Sin(Time.time * shakeSpeed) * shakeAmplitude : 0;
        transform.localEulerAngles = eulerAngle;

        closeBtn.SetActive(isShaking);
	}

    public void OnCloseBtnPressed() {
        viewController.removeView(selfNode);
    }

    public void makeQuery(IDictionary qry, JSEvalDelegate callback = null) {
        webView.EvaluateJavascript(string.Format("makeQuery({0})", UWKJson.Serialize(qry)), callback);
    }
}
