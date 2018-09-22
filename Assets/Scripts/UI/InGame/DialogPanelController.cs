using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogPanelController : MonoBehaviour {

	//variables
	[Range(0,10f)]
	[SerializeField] float timeSettings;
	[SerializeField] GameObject testManager;

	[SerializeField] bool isCorrect;
	[SerializeField] bool activation;

	[Header("UI")]
	[SerializeField] Sprite sprite_correct;
	[SerializeField] Sprite sprite_mistake;
	[SerializeField] Image icon;

	//components
	[Header("Audio")]
	AudioSource sfx_source;
	[SerializeField] AudioClip sfx_warning;
	[SerializeField] AudioClip sfx_correct;

	bool isCalm = true;

	InGameHelper inGameHelper;

	// Use this for initialization
	void OnEnable () {
		Debug.Log("Situational Dialog opened");

		inGameHelper = GameObject.Find("InGame SubManager").GetComponent<InGameHelper>();
		sfx_source = this.GetComponent<AudioSource>();
		isCorrect = testManager.GetComponent<testHandler>().isCorrect;

		var aiCount = GameObject.FindGameObjectsWithTag("AI");

		//play appropriate sound effect
		if(!isCorrect){

			icon.sprite = sprite_mistake;

			sfx_source.PlayOneShot(sfx_warning);

			if(aiCount.Length >= 3){

				// play intense music
				if(isCalm){

					inGameHelper.GetComponent<AudioSource>().Stop();
					inGameHelper.intenseMusicPlay();

					isCalm = false;

				}	

			}

		}else{

			icon.sprite = sprite_correct;
			sfx_source.PlayOneShot(sfx_correct);

			if(aiCount.Length <= 0){
				
				if(!isCalm){

					inGameHelper.GetComponent<AudioSource>().Stop();
					inGameHelper.playCalmTheme();

					isCalm = true;

				}
				
			}
		}

		StartCoroutine(closeTime(timeSettings));
	}
	
	// Update is called once per frame
	IEnumerator closeTime(float time){
		yield return new WaitForSeconds(time);
		gameObject.SetActive(false);
	}
}
