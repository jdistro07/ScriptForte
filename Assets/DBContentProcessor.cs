using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.SceneManagement;

public class DBContentProcessor : MonoBehaviour {

	[SerializeField] string link;

	[Header("Level Selection System")]
	public string testIndexID;
	public string testStringData; // will contain text from the database ready for pasring and shall be available to accessed publicly

	[Header("Test Credentials")]
	public string testCredentialData;
	
	[Header("Question")]
	public string questionData;

	[Header("Other Values")]
	public string testMode;

	public void OnClickTest(){

		testIndexID = EventSystem.current.currentSelectedGameObject.name;
		link = "http://"+gameObject.GetComponent<GameSettingsManager>().link+"/game_client/question_query.php";
		StartCoroutine(db_test_query(link, testIndexID));

	}

	IEnumerator db_test_query(string url, string test_index_ID){

		//launch loading scene
		LevelLoader("loading");

		//initialize www form/s and link url for question_query.php file
		WWWForm form = new WWWForm();

		form.AddField("test_ID_request",test_index_ID);

		WWW www = new WWW(url,form);
		Debug.Log("Processing test query for test "+test_index_ID+": "+url);

		//wait for the request to finish
		yield return www;

		//assign return value from the query
		testStringData = www.text;
		Debug.Log("Question query returned: "+www.text);

		//split testStringData to credentials credentials and questions
		string[] seperator = testStringData.Split(new string[] {"<ed>"}, StringSplitOptions.None);

		testCredentialData = seperator[0];
		questionData = seperator[1];

		//load InGame scene after the arrangement process
		//LevelLoader("InGame");

	}

	public void StartGame(){

		Initiate.Fade("InGame", Color.black, .2f);

	}

	//global scene loader
	public void LevelLoader(string scene_name){

		Initiate.Fade(scene_name, Color.black, .5f);

	}

	public void TestMode(string mode){

		switch(mode){

			case "PRE":

			testMode = "PRE";
			
			break;

			default:

			testMode = "POST";

			break;
		}

		Debug.Log("Test Mode: "+testMode);

	}

}
