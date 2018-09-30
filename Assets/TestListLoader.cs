using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestListLoader : MonoBehaviour {

	[Header("Components")]
	[SerializeField] AnimationClip MainUIPanelAnimation;
	[SerializeField] GameObject itemPanelPrefrab;

	[Header("Test")]
	[SerializeField] string[] tests;

	[Header("UI Elements References")]
	[SerializeField, Tooltip("Scroll rect content UI object where the test panel is being spawned as child.")] 
	GameObject ScrollContent;

	[SerializeField, Tooltip("Test prefab panel that spanws as a child to the Scroll rect content.")] 
	GameObject testItemPrefab;

	[Header("Test item panel elements")]
	[SerializeField] Text txtID;
	[SerializeField] Text txtTestName;
	[SerializeField] Text txtAuthor;
	[SerializeField] Text txtTestType;

	void OnEnable () {
		
		StartCoroutine(AfterAnimation());

	}

	IEnumerator AfterAnimation(){

		// wait for animation to finish since some gameobject might be innactive during animation
		// this is to avoid furthermor object reference errors
		// query questions that are built-in
		yield return new WaitForSeconds(MainUIPanelAnimation.length);

		//get link from configuration
		// start test list query
		string configured_link = GameObject.Find("AIOGameManager").GetComponent<GameSettingsManager>().link;

		StartCoroutine(QueryTest(configured_link));

	}

	public IEnumerator QueryTest(string link){

		LoginModule loginModule = GameObject.Find("AIOGameManager").GetComponent<LoginModule>(); //Login module reference

		string user_id = loginModule.userID;
		string username = loginModule.accountUsername;
		int accountLevel = int.Parse(loginModule.accountLevel);

		string phpLink = "http://"+link+"/game_client/query_test.php";

		// query tests with type "Built-in"
		WWWForm wwwfrom = new WWWForm();

		wwwfrom.AddField("test_type", "Built-in");
		
		WWW www = new WWW(phpLink, wwwfrom);

		Debug.Log("Requesting test list: "+phpLink);

		yield return www;

		string dataString = www.text;

		Debug.Log(dataString);

		// call the CredentialSeperator function from the LoginModule to seperate the information
		// sperate the tests return items with ":"

		tests = dataString.Split(new char[] {':'}, System.StringSplitOptions.RemoveEmptyEntries);

		// spawn test item prefab as child of the scroll rect content per test
		for(int i = 0; i != tests.Length; i++){

			var item = Instantiate(testItemPrefab) as GameObject;
			item.transform.SetParent(ScrollContent.transform, false);

			// sperate credentials while LoginModule CredentialSeperator Function
			// display it to the appropriate text element of each panel
			// Test information UI elements
			txtID = item.transform.GetChild(3).GetChild(0).GetComponent<Text>();
			txtTestType = item.transform.GetChild(3).GetChild(1).GetComponent<Text>();
			txtTestName = item.transform.GetChild(3).GetChild(2).GetComponent<Text>();
			txtAuthor = item.transform.GetChild(3).GetChild(3).GetComponent<Text>();

			// set UI values
			txtID.text = txtID.text+loginModule.CredentialSeperator(tests[i], "TestID=");
			txtTestName.text = txtTestName.text+" "+loginModule.CredentialSeperator(tests[i], "Name=");
			txtAuthor.text = txtAuthor.text+" "+loginModule.CredentialSeperator(tests[i], "Author=");
			txtTestType.text = txtTestType.text+" "+loginModule.CredentialSeperator(tests[i], "TestType=");

			yield return null;

		}

	}
	
}
