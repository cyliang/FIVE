using UnityEngine;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour {

	public Material red, blue;
	KeyCode code;

	// Use this for initialization
	void Start () {
		foreach (KeyCode vkey in System.Enum.GetValues(typeof(KeyCode))) {
			if (vkey.ToString () == name) {
				code = vkey;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (code)) {
			gameObject.GetComponentInChildren<SpriteRenderer> ().sprite = Resources.Load ("RedSquare", typeof(Sprite)) as Sprite;
			//gameObject.GetComponent<Renderer> ().material = red;

			Debug.Log ("Pressed");
		}
		if (Input.GetKeyUp (code)) {
			gameObject.GetComponentInChildren<SpriteRenderer> ().sprite = Resources.Load ("BlueSquare", typeof(Sprite)) as Sprite;
			//gameObject.GetComponent<Renderer> ().material = blue;

			Debug.Log ("Released");
		}
		/*if(Input.GetKeyDown())
		{
			gameObject.GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load("RedSquare", typeof(Sprite)) as Sprite;
			gameObject.GetComponent<Renderer> ().material = red;

			Debug.Log("Pressed");
		}
		if(Input.GetKeyUp(KeyCode.A))
		{
			gameObject.GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load("BlueSquare", typeof(Sprite)) as Sprite;
			gameObject.GetComponent<Renderer> ().material = blue;

			Debug.Log("Released");
		}*/
	
	}
	
}
