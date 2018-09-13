using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {

	[Header("Profile Panel")]
	[SerializeField] Text fullName;
	[SerializeField] Text user_grade_or_account_level;
	[SerializeField] Text overallConsistency;

	void OnEnable() {

		// get user credentials
		LoginModule user_credential = GameObject.Find("AIOGameManager").GetComponent<LoginModule>();

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

		// get consistency approximation (all scores added together from the DB / the count of the scores)

	}

}
