using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginModule : MonoBehaviour {

	[SerializeField]string link;

	[SerializeField] string input_username;
	string input_password;

	[Header("User Credentials")]
	public string acc_id;
	public string completeName;
	public string acc_level;

	[Header("Animation Components")]
	public AnimationClip loginTransition_Allow;
	[SerializeField]Animator loginAnimator;

	[Header("Gameobjects")]
	[SerializeField]GameObject loginCanvas;
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

	IEnumerator DB_Login(string username, string password){
		WWWForm form = new WWWForm();

		form.AddField("client_username",username);
		form.AddField("client_password",password);

		WWW www = new WWW(link,form);

		//disable button to avoid spam requests
		loginCanvas_btnLogin.enabled = false;

		Debug.Log("Processing request at "+link);
		yield return www;
		
		if(www.text == "Login granted!"){
			//Animate and disable canvas to open the main menu
			loginAnimator.SetBool("isLogged", true);
			StartCoroutine(disableLogin());
		}else if(www.text == "Login denied!" || www.text == "No username on the input!" || www.text == "No password on the input!"){
			loginCanvas_btnLogin.enabled = true;
			Debug.Log("Login failed: "+www.text);
		}else{
			loginCanvas_btnLogin.enabled = true;
			Debug.Log("Something went wrong: "+www.text);
		}
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
