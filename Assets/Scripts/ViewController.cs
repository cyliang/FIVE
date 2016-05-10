using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ViewController : MonoBehaviour {

    public GameObject viewPrefab;

    private LinkedList<GameObject> viewList = new LinkedList<GameObject>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.F1)) {
            createView();
        }
	}

    void createView() {
        GameObject newView = Instantiate(viewPrefab);
        newView.transform.parent = transform;
        viewList.AddLast(newView);
        rearrange();
    }

    void rearrange() {
        float firstAngle = - (viewList.Count-1) / 2f * 30;
        foreach(GameObject view in viewList) {
            view.transform.eulerAngles = new Vector3(0, firstAngle, 0);
            firstAngle += 30;
        }
    }
}
