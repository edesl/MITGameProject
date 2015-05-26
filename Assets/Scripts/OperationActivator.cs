using UnityEngine;
using System.Collections;

public class OperationActivator : MonoBehaviour {

	public GameObject dynamicPlatform;
	public GameObject numbersCanvas;

	public bool isAnswerCorrect = false;

	void OnTriggerEnter2D (Collider2D other){

//		Debug.Log ("Ha entrado");
//		dynamicPlatform.SetActive (true);
		numbersCanvas.SetActive (true);
	}

	void OnTriggerExit2D (Collider2D other){
//		Debug.Log ("Ha salido");
//		dynamicPlatform.SetActive (false);

		if (!isAnswerCorrect){
//			numbersCanvas.SetActive (false);
		}

	}

	void ResizePlatform (){
		Debug.Log ("Hola aqui");

		Vector3 test = new Vector3 (4f, 0.5f, 1f);
		dynamicPlatform.GetComponent<Transform>().localScale = test;
	}
}
