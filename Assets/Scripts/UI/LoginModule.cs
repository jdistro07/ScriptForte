using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginModule : MonoBehaviour {

	[Header("Animation Components")]
	public AnimationClip loginTransition_Allow;
	[SerializeField]Animator loginAnimator;

	[SerializeField]GameObject loginCanvas;

	[Header("Components")]
	public bool LoggedIn;

	void Start(){
		loginAnimator = loginCanvas.GetComponent<Animator>();
	}

	public void ClickLogin(){
		// WWW Login Logic Here

		//Animate and disable canvas to open the main menu
		loginAnimator.SetBool("isLogged", true);
		StartCoroutine(disableLogin());
	}

	IEnumerator disableLogin(){
		yield return new WaitForSeconds(loginTransition_Allow.length);
		loginCanvas.SetActive(false);
		
		if(loginCanvas.activeSelf == false){
			LoggedIn = true;
			Debug.Log("Login confirmed! Launching main menu...");
		}
	}
}
