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

	[SerializeField] GameObject MenuPanel;

	void OnEnable() {

		//enable main menu
		btnMenuCredits.SetActive(true);
		btnMenuTutorial.SetActive(true);
		btnMenuLearn.SetActive(true);
		MenuPanel.SetActive(true);

		// get user credentials
		user_credential = GameObject.Find("AIOGameManager").GetComponent<LoginModule>();
		

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

		StartCoroutine(QueryConsistency());

	}

	private void OnDisable()
	{
		//disable main menu
		btnMenuCredits.SetActive(false);
		btnMenuTutorial.SetActive(false);
		btnMenuLearn.SetActive(false);
		MenuPanel.SetActive(false);
	}

	IEnumerator QueryConsistency(){

		// get consistency approximation (all scores added together from the DB / the count of the scores)
		string link = GameObject.Find("AIOGameManager").GetComponent<GameSettingsManager>().link+"/game_client/query_consistency.php";

		Debug.Log(link);

		string username = user_credential.accountUsername;
		string accountID = user_credential.userID;

		WWWForm form = new WWWForm();

		form.AddField("client_username",username);
		form.AddField("client_user_ID",accountID);

		WWW www = new WWW(link,form);

		yield return www;

		Debug.Log(www.text);
		overallConsistency.text = www.text+" %";

	}

}
