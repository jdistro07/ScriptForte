using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelTestController : MonoBehaviour {

	[Header("Elements")]
	[SerializeField] Button btnPost = null;
	[SerializeField] Button btnPre = null;
	[SerializeField] Text txtTestID;
	[SerializeField] Text txtPreTestRate;
	[SerializeField] Text txtPostTestRate;

	[Header("Values")]
	[SerializeField] string[] credentials;

	LoginModule loginModule;

	// Use this for initialization
	private void Start()
	{

		//initilize gameobject references
		loginModule = GameObject.Find("AIOGameManager").GetComponent<LoginModule>();
		GameSettingsManager settings = GameObject.Find("AIOGameManager").GetComponent<GameSettingsManager>();
		UIManager ui_Manager = GameObject.Find("AIOGameManager").GetComponent<UIManager>();
		DBContentProcessor db_processor = GameObject.Find("AIOGameManager").GetComponent<DBContentProcessor>();

		// rename buttons to test ID for the DB processor to read
		credentials = txtTestID.text.Split(new char[] {'#'}, System.StringSplitOptions.RemoveEmptyEntries);
		btnPost.name = credentials[1];
		btnPre.name = credentials[1];

		StartCoroutine(q_HighestRating(settings.link, loginModule.accountUsername, int.Parse(loginModule.userID), int.Parse(credentials[1])));

		// add functionalities to pre and post buttons
		btnPost.onClick.AddListener(()=> {
			
			loginModule.themeMusicStop();

			ui_Manager.sfxOpen(); 
			db_processor.TestMode("POST");
			db_processor.OnClickTest();
			
			});

		btnPre.onClick.AddListener(()=> {
		
			loginModule.themeMusicStop();

			ui_Manager.sfxOpen(); 
			db_processor.TestMode("PRE");
			db_processor.OnClickTest();
			
			});

	}

	IEnumerator q_HighestRating(string link, string username, int userID, int testID){

		string phpLink = link+"/game_client/highest_score.php";

		// set highest rating for post test and pre test with username and test id from credentials[1]
		WWWForm wwwForm = new WWWForm();

		wwwForm.AddField("username",username);
		wwwForm.AddField("userID",userID);
		wwwForm.AddField("test_ID",testID);

		WWW www = new WWW(phpLink, wwwForm);

		yield return www;

		Debug.Log(www.text);

		string stringData = www.text;

		txtPreTestRate.text = loginModule.CredentialSeperator(stringData, "Rate_PreTest=");
		txtPostTestRate.text = loginModule.CredentialSeperator(stringData, "Rate_PostTest=");

	}
	
}
