using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogPanelController : MonoBehaviour {

	//variables
	[Range(0,10f)]
	[SerializeField] float timeSettings;
	[SerializeField] GameObject testManager;

	[SerializeField] bool isCorrect;
	[SerializeField] bool activation;

	//components
	[Header("Audio")]
	AudioSource sfx_source;
	[SerializeField] AudioClip sfx_warning;
	[SerializeField] AudioClip sfx_correct;

	// Use this for initialization
	void OnEnable () {
		Debug.Log("Situational Dialog opened");

		sfx_source = this.GetComponent<AudioSource>();
		isCorrect = testManager.GetComponent<testHandler>().isCorrect;

		//play appropriate sound effect
		if(!isCorrect){
			sfx_source.PlayOneShot(sfx_warning);
		}else{
			sfx_source.PlayOneShot(sfx_correct);
		}

		StartCoroutine(closeTime(timeSettings));
	}
	
	// Update is called once per frame
	IEnumerator closeTime(float time){
		yield return new WaitForSeconds(time);
		gameObject.SetActive(false);
	}
}
