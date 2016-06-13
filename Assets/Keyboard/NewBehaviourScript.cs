using UnityEngine;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour {

	public Material red, blue;
	KeyCode Temp;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		foreach (KeyCode vkey in System.Enum.GetValues(typeof(KeyCode))) {
			if (vkey.ToString () == name) {
				if (Input.GetKeyDown (vkey)) {
					gameObject.GetComponentInChildren<SpriteRenderer> ().sprite = Resources.Load ("RedSquare", typeof(Sprite)) as Sprite;
					//gameObject.GetComponent<Renderer> ().material = red;

					Debug.Log ("Pressed");
				}
				if (Input.GetKeyUp (vkey)) {
					gameObject.GetComponentInChildren<SpriteRenderer> ().sprite = Resources.Load ("BlueSquare", typeof(Sprite)) as Sprite;
					//gameObject.GetComponent<Renderer> ().material = blue;

					Debug.Log ("Released");
				}

			}
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
