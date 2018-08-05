using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockedGateController : MonoBehaviour {

	[Header("Components")]
	public InputField inputField;
	public Canvas canvas;

	//animations
	public Animator gateState;
	[SerializeField] Animator PanelAnimator;

	public AnimationClip quiz_close;

	//audio
	public AudioClip audioClip;

	public BoxCollider audioTrigger;

	AudioSource soundEffects;
	AudioSource canvasCloseSFX;
	

	[Header("Properties")]
	[SerializeField] string GatePasscode;
	public string PlayerPasscode;
	
	private void Start()
	{
		soundEffects = this.GetComponent<AudioSource>();
		PanelAnimator = canvas.GetComponent<Animator>();
		canvasCloseSFX = canvas.GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void LateUpdate () {

		PlayerPasscode = inputField.text;

		if(PlayerPasscode == GatePasscode){
			//open the gate
			gateState.SetInteger("open",2);
			
			if(!audioTrigger.enabled){
				StartCoroutine(canvasClose());
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

	IEnumerator canvasClose(){
		PanelAnimator.SetInteger("open", 1);
		canvasCloseSFX.Play();
		yield return new WaitForSeconds(quiz_close.length);
		canvas.enabled = false;
	}
}
