using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VideoLoader : MonoBehaviour {

	[SerializeField] string[] returnedList;

	[Header("Video Panel")]
	[SerializeField] GameObject videoItemPanel;
	[SerializeField] GameObject videoContentParent;

	// Use this for initialization
	void OnEnable () {

		string server_url = GameObject.Find("AIOGameManager").GetComponent<GameSettingsManager>().link;

		StartCoroutine(videoRequest(server_url));
		
	}

	private void OnDisable()
	{
		
		foreach(Transform child in videoContentParent.transform){

			Destroy(child.gameObject);

		}

	}

	IEnumerator videoRequest(string link){

		WWW www = new WWW(link+"/game_client/lesson_list.php");

		yield return www;
		Debug.Log(www.text);

		returnedList = www.text.Split(new string[] {"<br>"}, StringSplitOptions.RemoveEmptyEntries);

		foreach(string entry in returnedList){

			Instantiate(videoItemPanel).transform.SetParent(videoContentParent.transform);
			yield return null;

		}

	}
	
}
