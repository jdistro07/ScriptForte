using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class sceneTrigger : MonoBehaviour
{
	private string link;
	private string testType;

	[SerializeField] private string userID;
	[SerializeField] private string username;
	[SerializeField] private string testID;
	[SerializeField] private string rating;
	[SerializeField] private string testMode;

	private GameObject GameController;
	private testHandler testHandler;

	FirstPersonController FPC;

	private void Awake()
	{
		GameController = GameObject.FindGameObjectWithTag ("GameController");
		testHandler = GameObject.Find ("The Testing Ground").GetComponent<testHandler> ();
		testType = GameController.GetComponent<DBContentProcessor> ().testMode;
		FPC = GameObject.FindGameObjectWithTag ("Player").GetComponent<FirstPersonController> ();
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

			if (testType == "PRE")
			{
				FPC.m_MouseLook.lockCursor = false;
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;

				Initiate.Fade ("learn", Color.black, 5f);
			}
			else if (testType == "POST")
			{
				FPC.m_MouseLook.lockCursor = false;
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
				
				Initiate.Fade ("Main UI", Color.black, 5f);
			}
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
