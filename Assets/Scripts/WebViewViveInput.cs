using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class WebViewViveInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    public bool ViveControllerEnabled = true;

    private UWKWebView webView;
    private int lastX, lastY;

	// Use this for initialization
	void Start () {
        webView = GetComponent<UWKWebView>();
	}
	
	// Update is called once per frame
	void Update () {
        if (!ViveControllerEnabled)
            return;

        RaycastHit hit = ViveControllerInput.Instance.raycastHit;
        if (hit.transform == null || hit.collider.gameObject != gameObject)
            return;
        
        int x = (int)(hit.textureCoord.x * (float)webView.MaxWidth);
        int y = webView.MaxHeight - (int)(hit.textureCoord.y * (float)webView.MaxHeight);
        if (x != lastX || y != lastY) {
            UWKPlugin.UWK_MsgMouseMove(webView.ID, x, y);
            lastX = x;
            lastY = y;
        }

        /* TODO: scroll */
    }

    public void OnPointerDown(PointerEventData eventData) {
        UWKPlugin.UWK_MsgMouseButton(webView.ID, lastX, lastY, 0, true);
    }

    public void OnPointerUp(PointerEventData eventData) {
        UWKPlugin.UWK_MsgMouseButton(webView.ID, lastX, lastY, 0, false);
    }
}
