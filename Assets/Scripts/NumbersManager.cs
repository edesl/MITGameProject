using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NumbersManager : MonoBehaviour {

	public GameObject dynamicPlatform;

	public GameObject button2;
	public GameObject button3;

	public Text buttonAText;
	public int number;
	public bool isSecondNumber = false;


	private int number2;
	private bool optionsOn = false;

	void Start (){
		buttonAText.text = number.ToString ();
	}

	public void NumberOptions (){

		if (!optionsOn){
			button2.SetActive (true);
			button3.SetActive (true);
			optionsOn = true;
			buttonAText.text = number.ToString();
		}else{
			button2.SetActive (false);
			button3.SetActive (false);
			optionsOn = false;
		}


	}

	void AddNumber (int number){

		number2 = number;
	}

	void SetNumber (int number){

		buttonAText.text = number.ToString ();
		button2.SetActive (false);
		button3.SetActive (false);
		optionsOn = false;


		if (isSecondNumber){
			DynamicPlatform.numberB = number;
			dynamicPlatform.SendMessage ("Resize");
			dynamicPlatform.SendMessage ("Validate");
		}else {
			DynamicPlatform.numberA = number;
			dynamicPlatform.SendMessage ("Resize");
			dynamicPlatform.SendMessage ("Validate");
		
		}

	}
}
