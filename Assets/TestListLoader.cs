﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestListLoader : MonoBehaviour {

	[Header("Components")]
	[SerializeField] AnimationClip MainUIPanelAnimation;
	[SerializeField] GameObject itemPanelPrefrab;

	[Header("Test")]
	[SerializeField] string[] tests;
	public bool requestCustomTests = false;

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
		
		if(requestCustomTests){

			if(ScrollContent.activeInHierarchy == true){
				customTestLoader();
			}

		}else{
			StartCoroutine(AfterAnimation());
		}

	}

	void customTestLoader(){

		//get link from configuration
		// start test list query
		string configured_link = GameObject.Find("AIOGameManager").GetComponent<GameSettingsManager>().link;

		StartCoroutine(QueryTest(configured_link));

		Debug.Log("Requesting active custom tests");

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

	string truncateLongText(string text, int maxChar){
		return text.Length <= maxChar ? text : text.Substring(0, maxChar)+"...";
	}


	public IEnumerator QueryTest(string link){

		LoginModule loginModule = GameObject.Find("AIOGameManager").GetComponent<LoginModule>(); //Login module reference

		string user_id = loginModule.userID;
		string username = loginModule.accountUsername;
		int accountLevel = int.Parse(loginModule.accountLevel);

		string phpLink = "http://"+link+"/game_client/query_test.php";

		// query tests with type "Built-in"
		WWWForm wwwfrom = new WWWForm();

		// send request type
		if(requestCustomTests){
			wwwfrom.AddField("test_type", "Custom");
		}else{
			wwwfrom.AddField("test_type", "Built-in");
		}
		
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
			txtTestName.text = txtTestName.text+" "+truncateLongText(loginModule.CredentialSeperator(tests[i], "Name="), 25);
			txtAuthor.text = txtAuthor.text+" "+loginModule.CredentialSeperator(tests[i], "Author=");

			if(loginModule.CredentialSeperator(tests[i], "TestType=") == "Built-in"){
				txtTestType.text = "Chapter: "+" "+loginModule.CredentialSeperator(tests[i], "TestChapter=");
				item.GetComponent<PanelTestController>().test_chapter = int.Parse(loginModule.CredentialSeperator(tests[i], "TestChapter="));
			}else{
				txtTestType.text = "Type: "+" "+loginModule.CredentialSeperator(tests[i], "TestType=");
			}

			yield return null;

		}

	}
	
}
