using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {

	[Header("Script Reference")]
	[SerializeField]LoginModule user_credential;

	[Header("Profile Panel")]
	[SerializeField] Text fullName;
	[SerializeField] Text user_grade_or_account_level;
	[SerializeField] Text overallConsistency;

	[Header("Elements to show upon enable")]
	[SerializeField] GameObject btnMenuCredits;
	[SerializeField] GameObject btnMenuTutorial;
	[SerializeField] GameObject btnMenuLearn;
	[SerializeField] GameObject customTestList;
	[SerializeField] GameObject panelLoading;

	[Header("Password Panel")]
	[SerializeField] GameObject passwordPanel;
	[SerializeField] InputField txt_accountPassword;
	[SerializeField] InputField txt_newPassword;
	[SerializeField] InputField txt_confirmPassword;

	[SerializeField] GameObject MenuPanel;

	void OnEnable() {

		// get user credentials
		user_credential = GameObject.Find("AIOGameManager").GetComponent<LoginModule>();
		
		StartCoroutine(QueryConsistency());

		var user_level = int.Parse(user_credential.accountLevel);
		
		fullName.text = user_credential.fullName;
		
		// identify account level
		if(user_level == 1){

			Debug.Log("Admin account");
			user_grade_or_account_level.text = "Admin";

		}else if(user_level == 2){

			Debug.Log("Teacher account");
			user_grade_or_account_level.text = "Teacher";

		}else{

			// request the class section information of the student
			Debug.Log("Student account.");
			user_grade_or_account_level.text = "Grade: "+user_credential.gradeLevel;

		}

	}

	public void updatePassword(){
		passwordPanel.SetActive(true);
	}

	public void closeUpdatePassword(){
		passwordPanel.SetActive(false);

		// clean up values on disable
		if(!passwordPanel.activeInHierarchy){
			txt_accountPassword.text = string.Empty;
			txt_newPassword.text = string.Empty;
			txt_confirmPassword.text = string.Empty;
		}
	}

	private void OnDisable()
	{

		//disable main menu
		btnMenuCredits.SetActive(false);
		btnMenuTutorial.SetActive(false);
		btnMenuLearn.SetActive(false);
		MenuPanel.SetActive(false);
		customTestList.SetActive(false);
		
	}

	IEnumerator QueryConsistency(){

		// get consistency approximation (all scores added together from the DB / the count of the scores)
		string link = "http://"+GameObject.Find("AIOGameManager").GetComponent<GameSettingsManager>().link+"/game_client/query_consistency.php";

		//Debug.Log(link);

		panelLoading.SetActive(true);

		Text loadingText = panelLoading.transform.Find("Panel").Find("Loading_Text").GetComponent<Text>();
		loadingText.text = "Fetching information";

		string username = user_credential.accountUsername;
		string accountID = user_credential.userID;

		WWWForm form = new WWWForm();

		form.AddField("client_username",username);
		form.AddField("client_user_ID",accountID);

		WWW www = new WWW(link,form);

		yield return www;

		//enable main menu
		btnMenuCredits.SetActive(true);
		btnMenuTutorial.SetActive(true);
		btnMenuLearn.SetActive(true);
		MenuPanel.SetActive(true);
		customTestList.SetActive(true);

		// disable loading
		panelLoading.SetActive(false);

		Debug.Log(www.text);
		overallConsistency.text = www.text+" %";

		// get latest max chapter finished by the player
		string link_maxChapter = "http://"+GameObject.Find("AIOGameManager").GetComponent<GameSettingsManager>().link+"/game_client/query_maxChapter.php";

		WWWForm credential_form = new WWWForm();

		credential_form.AddField("user_ID", accountID);

		WWW req_maxChap = new WWW(link_maxChapter, credential_form);

		yield return req_maxChap;

		// store max chapter value as integer to the public maxChapter variable of the login module
		user_credential.maxChapter = req_maxChap.text;

	}

}
