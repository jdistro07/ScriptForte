using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {

	[Header("Profile Panel")]
	[SerializeField] Text fullName;
	[SerializeField] Text user_grade_or_account_level;
	[SerializeField] Text overallConsistency;

	void Start() {

		// get user credentials
		LoginModule user_credential = GameObject.Find("AIOGameManager").GetComponent<LoginModule>();
		
		fullName.text = user_credential.fullName;
		
		// identify account level
		if(user_credential.accountLevel == "1"){

			Debug.Log("Admin account");
			user_grade_or_account_level.text = "Admin";

		}else if(user_credential.accountLevel == "2"){

			Debug.Log("Teacher account");
			user_grade_or_account_level.text = "Teacher";

		}else{

			Debug.Log("Student account, requesting grade level to the server");
			

		}

	}

}
