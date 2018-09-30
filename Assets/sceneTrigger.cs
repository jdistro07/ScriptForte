using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sceneTrigger : MonoBehaviour
{
	private string link;

	[SerializeField] private string userID;
	[SerializeField] private string username;
	[SerializeField] private string testID;
	[SerializeField] private string rating;
	[SerializeField] private string testMode;

	private GameObject GameController;
	private testHandler testHandler;

	private void Awake()
	{
		GameController = GameObject.FindGameObjectWithTag ("GameController");
		testHandler = GameObject.Find ("The Testing Ground").GetComponent<testHandler> ();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.tag == "Player")
		{
			userID = GameController.GetComponent<LoginModule> ().userID;
			username = GameController.GetComponent<LoginModule> ().accountUsername;
			testID = GameController.GetComponent<DBContentProcessor> ().testIndexID;
			rating = testHandler.scoreAverage.ToString ();
			testMode = GameController.GetComponent<DBContentProcessor> ().testMode;

			StartCoroutine(submitScore (userID, username, testID, rating, testMode));

			Initiate.Fade ("learn", Color.black, 5f);
		}
	}

	private IEnumerator submitScore(string sf_userID, string sf_username, string sf_testID, string sf_rating, string sf_testMode)
	{
		WWWForm form = new WWWForm ();
		form.AddField ("sf_userID", sf_userID);
		form.AddField ("sf_username", sf_username);
		form.AddField ("sf_testID", sf_testID);
		form.AddField ("sf_rating", sf_rating);
		form.AddField ("sf_testMode", sf_testMode);

		link = "http://" + GameController.GetComponent<GameSettingsManager> ().link + "/game_client/score_submit.php";
		WWW www = new WWW (link, form);

		yield return www;

		Debug.Log(www.text);
	}
}
