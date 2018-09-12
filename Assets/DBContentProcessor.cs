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
		LevelLoader("InGame");

	}

	//global scene loader
	public void LevelLoader(string scene_name){

		StartCoroutine(LoadScene(scene_name));

	}

	IEnumerator LoadScene(string scene_name){

		AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scene_name);

		// debug loading status after a frame and clamp the progress to 1 for scene activation completing the whole loading process
		while(!asyncOperation.isDone){

			float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
			Debug.Log(scene_name+" = "+progress);

			yield return null;

		}

	}
}
