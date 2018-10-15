using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;

public class VideoLoader : MonoBehaviour {

	[SerializeField] string[] returnedList;

	[Header("Video Panel")]
	[SerializeField] GameObject videoItemPanel;
	[SerializeField] GameObject videoContentParent;

	LoginModule login_mod;

	// Use this for initialization
	void OnEnable () {

		string server_url = GameObject.Find("AIOGameManager").GetComponent<GameSettingsManager>().link;
		login_mod = GameObject.Find("AIOGameManager").GetComponent<LoginModule>();

		StartCoroutine(videoRequest(server_url));
		
	}

	private void OnDisable()
	{
		
		foreach(Transform child in videoContentParent.transform){

			Destroy(child.gameObject);

		}

	}

	IEnumerator videoRequest(string link){

		WWWForm wwwform = new WWWForm();

		wwwform.AddField("username", login_mod.accountUsername);
		wwwform.AddField("user_id", login_mod.userID);

		WWW www = new WWW("http://"+link+"/game_client/lesson_list.php", wwwform);

		yield return www;
		Debug.Log(www.text);

		returnedList = www.text.Split(new string[] {"<br>"}, StringSplitOptions.RemoveEmptyEntries);

		for(int i = 0; i != returnedList.Length; i++){

			var item = Instantiate(videoItemPanel) as GameObject;
			item.transform.SetParent(videoContentParent.transform, false);
			
			// give the target video to the panel
			item.GetComponent<item_video>().videoTarget = returnedList[i];

			// check if the naming convention is valid
			if(returnedList[i].Substring(0,1) == "["){

				// decode the file name and set the value of the test ID
				Debug.Log("Decoding "+returnedList[i]);

				Regex regex = new Regex(@"\[([^]])\]");
				MatchCollection mc = regex.Matches(returnedList[i]);

				Debug.Log(mc[0].Value.Replace("[", "").Replace("]", ""));

				item.GetComponent<item_video>().testid = int.Parse(mc[0].Value.Replace("[", "").Replace("]", ""));
				

			}

			// set controls
			Text lessonName = item.transform.GetChild(2).GetChild(0).GetComponent<Text>();

			// display values
			lessonName.text = returnedList[i].Replace(".mp4", string.Empty);

			yield return null;

		}

	}
	
}
