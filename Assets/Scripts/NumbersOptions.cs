using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NumbersOptions : MonoBehaviour {

	public GameObject mainNumber;

	public int number;
	public Text buttonText;

	void Start (){

		buttonText.text = number.ToString ();
	}


	public void SendNumber (){
		mainNumber.SendMessage ("SetNumber",number);
	}
}
