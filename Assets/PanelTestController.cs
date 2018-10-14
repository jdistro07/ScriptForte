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
	[SerializeField] int pre_playCount;

	public string stringData;

	TestListLoader testListLoader;

	LoginModule loginModule;

	// Use this for initialization
	private void Start()
	{
		
		//initilize gameobject references
		loginModule = GameObject.Find("AIOGameManager").GetComponent<LoginModule>();
		GameSettingsManager settings = GameObject.Find("AIOGameManager").GetComponent<GameSettingsManager>();
		UIManager ui_Manager = GameObject.Find("AIOGameManager").GetComponent<UIManager>();
		DBContentProcessor db_processor = GameObject.Find("AIOGameManager").GetComponent<DBContentProcessor>();

		// lock post-test buttons by default
		btnPost.interactable = false;
		btnPost.GetComponentInChildren<Text>().text = "LOCKED";
		btnPost.GetComponentInChildren<Text>().color = Color.red;

		// rename buttons to test ID for the DB processor to read
		credentials = txtTestID.text.Split(new char[] {'#'}, System.StringSplitOptions.RemoveEmptyEntries);
		btnPost.name = credentials[1];
		btnPre.name = credentials[1];

		StartCoroutine(q_HighestRating("http://"+settings.link, loginModule.accountUsername, int.Parse(loginModule.userID), int.Parse(credentials[1])));

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

		stringData = www.text;

		// fetch values for control references
		pre_playCount = int.Parse(loginModule.CredentialSeperator(stringData, "TestPlayCount="));

		//assign values for operations
		txtPreTestRate.text = loginModule.CredentialSeperator(stringData, "Rate_PreTest=");
		txtPostTestRate.text = loginModule.CredentialSeperator(stringData, "Rate_PostTest=");

		panelContent_unlocker(pre_playCount);

	}

	public void panelContent_unlocker(int pre){

		// get component for the test list loader
		try{
			testListLoader = transform.parent.GetComponent<TestListLoader>();

			if(testListLoader.requestCustomTests){
				btnPost.interactable = true;
				btnPost.GetComponentInChildren<Text>().text = "Take Test";
				btnPost.GetComponentInChildren<Text>().color = Color.white;
			}

		}catch{

			// pre-test unlock logic
			if(pre > 0){
				btnPost.interactable = true;
				btnPost.GetComponentInChildren<Text>().text = "Post-test";
				btnPost.GetComponentInChildren<Text>().color = Color.white;
			}

		}

	}
	
}
