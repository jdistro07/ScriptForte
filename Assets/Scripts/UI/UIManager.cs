using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	[Header("Main UI Component")]
	public GameObject loginPanel;

	[Header("Universal UI Component")]
	public GameObject gameSettingsPanel;

	[Header("Sound Effects")]
	public AudioSource UIAudioSource;
	public AudioClip openSfx;
	public AudioClip closeSfx;
	public AudioClip specialSfx;

	// sound effects UI
	public void sfxOpen(){

		UIAudioSource.PlayOneShot(openSfx);

	}

	public void sfxClose(){

		UIAudioSource.PlayOneShot(closeSfx);

	}

	public void sfxSpecial(){

		UIAudioSource.PlayOneShot(specialSfx);

	}

	//universal methods
	public void OpengameSettings(){

		loginPanel.SetActive(false);
		gameSettingsPanel.SetActive(true);

	}

	public void ClosegameSettings(){

		var loginCheck = this.GetComponent<LoginModule>();

		if(!loginCheck.LoggedIn){

			loginPanel.SetActive(true);
			
		}
		
		gameSettingsPanel.SetActive(false);

	}
	//universal methods END

	public void OnApplicationQuit(){

		Debug.Log("Application Close");
		Application.Quit();


	}
	
}
