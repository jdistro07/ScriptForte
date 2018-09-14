using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class TestListLoader : MonoBehaviour {

	[Header("Components")]
	[SerializeField] AnimationClip MainUIPanelAnimation;
	[SerializeField] GameObject itemPanelPrefrab;

	[Header("Test")]
	[SerializeField] string[] tests;

	[Header("UI Elements")]
	[SerializeField] GameObject ScrollContent;
	[SerializeField] GameObject testItemPrefab;
	

	void Start () {
		
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

		string phpLink = link+"/game_client/query_test.php";

		// query tests with type "Built-in"
		WWWForm wwwfrom = new WWWForm();

		wwwfrom.AddField("test_type_request", "Built-in");
		
		WWW www = new WWW(phpLink, wwwfrom);

		Debug.Log("Requesting test list: "+phpLink);

		yield return www;

		string dataString = www.text;

		Debug.Log(dataString);

		// call the CredentialSeperator function from the LoginModule to seperate the information
		// sperate the tests return items with ":"
		LoginModule loginModule = GameObject.Find("AIOGameManager").GetComponent<LoginModule>();

		tests = dataString.Split(new char[] {':'}, System.StringSplitOptions.RemoveEmptyEntries);

		// spawn test item prefab as child of the scroll rect content per test
		for(int i = 0; i != tests.Length; i++){

			var item = Instantiate(testItemPrefab) as GameObject;
			item.transform.parent = ScrollContent.transform;

		}


	}
	
}
