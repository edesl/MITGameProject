using UnityEngine;
using System.Collections;

public class DynamicPlatform : MonoBehaviour {

	public int correctAnswer;

	public static int numberA;
	public static int numberB;

	private Renderer myRenderer;
	private Transform myTransform;
	private int result = 0;
	private Rigidbody2D myRigidbody2D;
	private bool isFall = false;

	void Start () {

		myRigidbody2D = GetComponent<Rigidbody2D>();
		myTransform = GetComponent<Transform>();
		myRenderer = GetComponent<Renderer>();
		numberA = 0;
		numberB = 0;
		isFall = false;
	
	}

	void Update () {

		myRenderer.material.SetTextureScale ("_MainTex", new Vector2(transform.localScale.x * 0.5f,1));
	
	}

	void OnTriggerEnter2D (Collider2D other){

		if (isFall){
			myRigidbody2D.isKinematic = false;
		}else {
			myRigidbody2D.isKinematic = true;
		}

	}

	
	void Resize (){
		result = numberA + numberB;
		myTransform.localScale = new Vector3 (result * 2, myTransform.localScale.y, myTransform.localScale.z);
	}
	
	void Validate (){

		if (result == correctAnswer){
			Debug.Log ("TODO OK: "+result);
			isFall = false;
		}else {
			Debug.Log ("AQUI CAE LA PLATAFORMA: "+result);
//			Invoke ("Fall", 1f);
			isFall = true;
		}
	}

	void Fall (){
		myRigidbody2D.isKinematic = false;
	}
}
