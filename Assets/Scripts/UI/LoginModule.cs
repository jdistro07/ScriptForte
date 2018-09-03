﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class LoginModule : MonoBehaviour {

	[SerializeField]string link;

	[SerializeField] string input_username;
	string input_password;

	[Header("Login Data Organization Processor")]
	[SerializeField] string dataString;
	[SerializeField] string[] splitDataString;

	[Header("User Processed Credentials")]
	public string userID;
	public string accountUsername;
	public string fullName;
	public string accountLevel;
	

	[Header("Animation Components")]
	public AnimationClip loginTransition_Allow;
	[SerializeField]Animator loginAnimator;

	[Header("Gameobjects")]
	[SerializeField]GameObject loginCanvas;
	[SerializeField]GameObject mainMenu;
	[SerializeField]InputField loginCanvas_username;
	[SerializeField]InputField loginCanvas_password;
	[SerializeField]Button loginCanvas_btnLogin;

	[Header("Components")]
	public bool LoggedIn;

	void Start(){

		loginAnimator = loginCanvas.GetComponent<Animator>();

	}

	public void ClickLogin(){

		// WWW Login Logic
		link = "http://"+gameObject.GetComponent<GameSettingsManager>().link+"/game_client/client_login.php";
		input_username = loginCanvas_username.text;
		input_password = loginCanvas_password.text;

		StartCoroutine(DB_Login(input_username, input_password));

	}

	public void ClickLogout(){

		loginCanvas.SetActive(true);
		StartCoroutine(enableLogin());

	}

	IEnumerator DB_Login(string username, string password){

		WWWForm form = new WWWForm();

		form.AddField("client_username",username);
		form.AddField("client_password",password);

		WWW www = new WWW(link,form);

		//disable button to avoid spam requests and enable if login process has an error
		loginCanvas_btnLogin.enabled = false;

		Debug.Log("Processing request at "+link);
		yield return www;

		//split the values. 1 for login 1 for the string data
		dataString = www.text;
		splitDataString = dataString.Split(':');
		
		if(splitDataString[0] == "Login granted"){

			//Grant login access Animate and disable canvas to open the main menu
			loginAnimator.SetInteger("LogState", 2);
			StartCoroutine(disableLogin());

			//set the user credentials
			userID = CredentialSeperator(splitDataString[1], "ID=");
			fullName = CredentialSeperator(splitDataString[1], "Name=");
			accountUsername = CredentialSeperator(splitDataString[1], "Username=");
			accountLevel = CredentialSeperator(splitDataString[1], "AccountLevel=");

			Debug.Log("Login granted: " + www.text);

		}else if(splitDataString[0] == "Login denied"){

			loginCanvas_btnLogin.enabled = true;
			Debug.Log("Login denied: " + www.text);

		}else{

			loginCanvas_btnLogin.enabled = true;
			Debug.Log("Something went wrong: " + www.text);

		}
	}

	string CredentialSeperator(string data_text, string index_category){
		
		//seperate credentials into specific indexes seperated by ":"
		string processedString = data_text.Substring(data_text.IndexOf(index_category) + index_category.Length);
		
		if(processedString.Contains("|")){

			processedString = processedString.Remove(processedString.IndexOf('|'));

		}

		return processedString;

	}

	IEnumerator disableLogin(){

		yield return new WaitForSeconds(loginTransition_Allow.length);
		loginCanvas.SetActive(false);
		
		if(loginCanvas.activeSelf == false){

			LoggedIn = true;
			Debug.Log("Login confirmed! Launching main menu...");

			mainMenu.SetActive(true);

		}
	}

	IEnumerator enableLogin(){

		loginAnimator.SetInteger("LogState", 3);
		mainMenu.SetActive(false);
		LoggedIn = false;

		yield return new WaitForSeconds(loginTransition_Allow.length);
		loginAnimator.SetInteger("LogState", 1);
		
		if(loginCanvas_btnLogin.enabled == false){

			//enable login button if disabled
			loginCanvas_btnLogin.enabled = true;

		}
		//loginAnimator.SetInteger("LogState", 1);

	}
}
