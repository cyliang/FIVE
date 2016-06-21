using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ViewBehavior : MonoBehaviour {
    
    public UWKWebView webView;
    public GameObject closeBtn;
    public GameObject selectedCircle;
    public float shakeSpeed, shakeAmplitude;

    public bool isShaking { get; set; }
    public bool selected { set { selectedCircle.SetActive(value); } }
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
    
    public void btnClicked(string type) {
        switch (type) {
            case "new":
                /* TODO */
                break;

            case "open":
                FileManager.fileBrowserCallback = fileOpened.loadFile;
                FileManager.fileBrowserStatus = FileManager.FileBrowserStatus.File;
                break;

            case "save":
                fileOpened.saveFile();
                break;
        }
    }

    public void OnCloseBtnPressed() {
        viewController.removeView(selfNode);
    }

    public void enlargeFont(int enlargement) {
        Dictionary<string, object> qry = new Dictionary<string, object>();
        qry.Add("cmd", "enlargeFont");
        qry.Add("params", enlargement);

        makeQuery(qry, (bool success, string response) => {
            if (!success)
                Debug.LogError(response);
        });
    }

    public void scroll(int lines) {
        Dictionary<string, object> qry = new Dictionary<string, object>();
        qry.Add("cmd", "navLines");
        qry.Add("params", lines);

        makeQuery(qry, (bool success, string response) => {
            if (!success)
                Debug.LogError(response);
        });
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
