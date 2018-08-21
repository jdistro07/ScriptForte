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

	private void Start()
	{
		
	}

	//universal methods
	public void sfxOpen(){
		UIAudioSource.PlayOneShot(openSfx);
	}

	public void sfxClose(){
		UIAudioSource.PlayOneShot(closeSfx);
	}

	public void sfxSpecial(){
		UIAudioSource.PlayOneShot(specialSfx);
	}
	//universal methods END

	//universal methods
	public void OpengameSettings(){
		loginPanel.SetActive(false);
		gameSettingsPanel.SetActive(true);
	}

	public void ClosegameSettings(){
		loginPanel.SetActive(true);
		gameSettingsPanel.SetActive(false);
	}
	//universal methods END
	
}
