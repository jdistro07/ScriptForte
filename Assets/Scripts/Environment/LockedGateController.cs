using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockedGateController : MonoBehaviour {

	[Header("Components")]
	[SerializeField] bool sfxPlayed;
	public InputField inputField;
	public Animator gateState;

	//audio
	public AudioClip audioClip;
	public BoxCollider audioTrigger;
	AudioSource soundEffects;
	

	[Header("Properties")]
	[SerializeField] string GatePasscode;
	public string PlayerPasscode;
	
	private void Start()
	{
		soundEffects = this.GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void LateUpdate () {
		PlayerPasscode = inputField.text;

		if(PlayerPasscode == GatePasscode){
			//open the gate
			gateState.SetInteger("open",2);
			
			if(!audioTrigger.enabled){
				audioTrigger.enabled = true;
			}
		}else if(PlayerPasscode != GatePasscode){ // future fix
			//gateState.SetInteger("open",1);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.transform.tag == "Player"){
			soundEffects.PlayOneShot(audioClip);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		audioTrigger.enabled = false;
	}
}
