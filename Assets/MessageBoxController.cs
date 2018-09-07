using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBoxController : MonoBehaviour {

	[Header("Information type")]
	public bool warning = false;
	public bool information = false;

	[Header("Sprites")]
	[SerializeField] Sprite sprite_warning;
	[SerializeField] Sprite sprite_information;
	[SerializeField] Sprite sprite_error;

	[SerializeField,Header("Components")]
	Text headerTextComponent;
	[SerializeField] Text bodyTextComponent;
	[SerializeField] Image messageIconComponent;

	void Update () {
		
		//evaluate message type
		if(warning){

			if(information){
				information = false;
			}

			//display warning components
			headerTextComponent.text = "WARNING";
			messageIconComponent.sprite = sprite_warning;
			

		}else if(information){
			
			if(warning){
				warning = false;
			}

			//display information warning components
			headerTextComponent.text = "INFORMATION";
			messageIconComponent.sprite = sprite_information;



		}else{

			if(warning || information){

				//disable everything if both warning and information is on
				warning = false;
				information = false;

			}

			// display error components
			headerTextComponent.text = "ERROR!";
			messageIconComponent.sprite = sprite_error;

		}

	}

	public void onClickOK(){

		gameObject.SetActive(false);

	}

}
