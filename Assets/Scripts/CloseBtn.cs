using UnityEngine;
using System.Collections;

public class CloseBtn : MonoBehaviour {

    public ViewBehavior parentView;

    void OnMouseDown() {
        parentView.OnCloseBtnPressed();
    }
}
