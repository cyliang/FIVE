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

    class QueryQueueData {
        public IDictionary qry;
        public JSEvalDelegate callback;
    }
    Queue<QueryQueueData> queryQueue = new Queue<QueryQueueData>();
    bool queryReady = false;

	// Use this for initialization
	void Start () {
        isShaking = false;
        webView.WebQuery += processWebQuery;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 eulerAngle = transform.localEulerAngles;
        eulerAngle.z = isShaking ? Mathf.Sin(Time.time * shakeSpeed) * shakeAmplitude : 0;
        transform.localEulerAngles = eulerAngle;
        closeBtn.SetActive(isShaking);

        if (queryReady && queryQueue.Count > 0) {
            var qry = queryQueue.Dequeue();
            webView.EvaluateJavascript(string.Format("makeQuery({0})", UWKJson.Serialize(qry.qry)), qry.callback);
        }
    }

    public void OnCloseBtnPressed() {
        viewController.removeView(selfNode);
    }

    public void makeQuery(IDictionary qry, JSEvalDelegate callback = null) {
        queryQueue.Enqueue(new QueryQueueData {
            qry = qry,
            callback = callback
        });
    }

    void processWebQuery(UWKWebQuery webQry) {
        if (webQry.Request == "ready") {
            queryReady = true;
        } else {
            Debug.Log(webQry.Request);
        }
    }
}
