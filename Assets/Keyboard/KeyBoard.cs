using UnityEngine;
using System.Collections;

public class KeyBoard : MonoBehaviour {
	// Use this for initialization
	public GameObject Model;
	void Start () {
		gameObject.SetActive (false);
		MenuController.addBtn("Show KeyBoard", () => {
			if(!gameObject.activeSelf){
				gameObject.SetActive (true);
				Model.SetActive (false);
			}
			else{
				gameObject.SetActive (false);
				Model.SetActive (true);
			}
		});
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
