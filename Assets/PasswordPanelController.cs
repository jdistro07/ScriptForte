using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PasswordPanelController : MonoBehaviour {

	[Header("Password inputs")]
	[SerializeField] InputField txt_accPassword;
	[SerializeField] InputField txt_newPassword;
	[SerializeField] InputField txt_confirmNewPassword;
	[SerializeField] Button btnUpdate;
	[SerializeField] Button btnPanelClose;

	// scripts
	GameSettingsManager gsettings;
	LoginModule login_module;

	public void pwUpdate(){

		gsettings = GameObject.Find("AIOGameManager").GetComponent<GameSettingsManager>();
		login_module = GameObject.Find("AIOGameManager").GetComponent<LoginModule>();

		// update password from the database
		StartCoroutine(dbPassUpdate(gsettings.link, txt_accPassword.text, txt_newPassword.text, txt_confirmNewPassword.text));
	}

	IEnumerator dbPassUpdate(string settingsLink, string accPassword, string newPassword, string confPassword){

		// get link for password update module
		string link = "http://"+settingsLink+"/game_client/update_password.php";

		WWWForm passwordForm = new WWWForm();

		// check if new password and confirm new password exactly match
		if(txt_accPassword.text != string.Empty && txt_newPassword.text != string.Empty && txt_confirmNewPassword.text != string.Empty && txt_newPassword.text.Length > 8){

			if(newPassword == confPassword){

				// get user credentials for user update reference
				string userID = login_module.userID;
				string username = login_module.accountUsername;
				int accLevel = int.Parse(login_module.accountLevel);

				// send appropriate data for update
				passwordForm.AddField("accPassword", accPassword);
				passwordForm.AddField("accID", userID);
				passwordForm.AddField("accUsername", username);
				passwordForm.AddField("accLevel", accLevel);

				passwordForm.AddField("newPassword", newPassword);
				
				WWW wwwRequest = new WWW(link, passwordForm);

				// disable controls that may interrupt the process
				btnUpdate.interactable = false;
				//btnUpdate.GetComponent<Text>().text = "Processing requests...";

				btnPanelClose.interactable = false;

				yield return wwwRequest;

				Debug.Log(wwwRequest.text);

				// display response after processing the request
				if(wwwRequest.text == "success"){

					// display success message
					login_module.messagePrompt("Account password updated successfully!", 2);
					gameObject.SetActive(false);


				}else if(wwwRequest.text == "mismatch"){

					// display password mismatch message
					login_module.messagePrompt("Invalid account password!", 1);

				}else{

					// display failure message
					login_module.messagePrompt("Account password updated failed!", 1);

				}

				btnUpdate.interactable = true;
				//btnUpdate.GetComponent<Text>().text = "Update Password";

				btnPanelClose.interactable = true;

			}else{

				// display error message box and clear values to all textbox on the password panel
				login_module.messagePrompt("Confirm password doesn't match!", 1);

			}

		}else{

			if (txt_newPassword.text.Length < 8){
				login_module.messagePrompt("Password should be at least 8 characters long!", 1);
			}else{
				login_module.messagePrompt("Fill the required fields!", 1);
			}

		}
		
		txt_accPassword.text = null;
		txt_newPassword.text = null;
		txt_confirmNewPassword.text = null;
	}

}
