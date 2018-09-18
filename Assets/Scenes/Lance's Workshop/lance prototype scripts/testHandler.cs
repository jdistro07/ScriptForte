using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class testHandler : MonoBehaviour
{
	[Header("Database Data")]
	[SerializeField] private string link;
	[SerializeField] private GameObject GameController;
	[SerializeField] private string questionsFetch;

	[SerializeField] private string testType;
	[SerializeField, Range(0, 100)] private float fetchLimit;

	[SerializeField] private string userID;
	[SerializeField] private string username;
	[SerializeField] private string testID;
	[SerializeField] private string rating;
	[SerializeField] private string testMode;

	[Header("Game Prefabs")]
	[SerializeField] private GameObject player;
	[SerializeField] private GameObject BotAI;
	[SerializeField] private GameObject healthBox;
	[SerializeField] private GameObject testPrefab_TF;
	[SerializeField] private GameObject testPrefab_MC;

	[Header("Player")]
	[SerializeField] public float maxPlayerHealth;
	[SerializeField] public float playerHealth;

	[Header("Player UI Components")]
	[SerializeField] GameObject DialogPanel;
	public Text DialogTitle;
	public Text DialogMessage;
	[SerializeField] RawImage DialogIcon;
	
	[Header("Platform Spawn Positions")]
	[SerializeField] private Transform platSpawnA;
	[SerializeField] private Transform platSpawnB;
	[SerializeField] private Transform platSpawnC;
	[SerializeField] private Transform platSpawnT;
	[SerializeField] private Transform platSpawnF;

	[Header("Test Questions")]
	[SerializeField] private List<String> questions;
	[SerializeField] private string[] test;

	[Header("Scoring")]
	[SerializeField] private int questionNumber = 0;
	public double totalQuestions;
	public double playerScore = 0;
	public double scoreAverage;

	[Header("AI")]
	[SerializeField] private GameObject[] bots;
	[SerializeField] private float botHeightOffset = 10;
	[SerializeField] private int maxBots = 4;
	private int botCount;

	[Header("Timer Components")]
	[Range(0,30)]
	public float timeToAdd;
	bool isTimeAdded;

	[Range(0,60)]
	public float time;

	public bool isCorrect;

	private Text questionText;

	private bool firstCreation = true;
	private bool isCreated = false;
	private bool botSpawned = false;
	private bool healthSpawned = false;
	private bool notified = false;

	[Header("Miscellaneous")]
	private Vector3 currentObjectSpawnPoint;
	private Vector3 newObjectSpawnPoint;
	[SerializeField] private float platformSpawnOffset = 40;
	[SerializeField] private GameObject testPlatform;
	[SerializeField] private GameObject currentPlatform;
	[SerializeField] private GameObject closest;
	[SerializeField] private Transform healthBox_spawn;
	[SerializeField] private float healthBox_yOffset;
	private float rand;

	private bool playerCheck;
	TestingGroundMonitor playerCanvas;


	private void Start()
	{
		GameObject playerFind = GameObject.Find ("FPSController");
		Vector3 playerSpawn = GameObject.Find ("PlayerSpawn").GetComponent<Transform> ().position;
		Quaternion playerRotation = GameObject.Find ("PlayerSpawn").GetComponent<Transform> ().rotation;

		GameObject.Find ("Start_End Platform").transform.Find ("UI Components").Find ("UICanvas").gameObject.SetActive(false);

		if (!playerFind)
		{
			Instantiate (player, playerSpawn, playerRotation);
		}

		playerCanvas = GameObject.Find("PlayerUI_Canvas").GetComponent<TestingGroundMonitor>();

	}

	private void Awake()
	{
		GameController = GameObject.FindGameObjectWithTag("GameController");
		testType = GameController.GetComponent<DBContentProcessor> ().testMode;
		questionsFetch = GameController.GetComponent<DBContentProcessor> ().questionData;

		test = questionsFetch.Split (new String[] {"~"}, StringSplitOptions.RemoveEmptyEntries);

		if (testType == "PRE")
		{
			for (int x = 0; x < Mathf.Round(test.Length * (fetchLimit / 100)) ; x++)
			{
				questions.Add (test [x]);
			}
		}
		else if (testType == "POST")
		{
			for (int x = 0; x < test.Length; x++)
			{
				questions.Add (test [x]);
			}
		}

		Debug.Log ("total number of questions added: " + questions.Count);
		totalQuestions = questions.Count;
	}

	private void Update()
	{
		currentObjectSpawnPoint = GameObject.Find ("testPlatform_Spawn_Start").GetComponent<Transform> ().transform.position;
		currentObjectSpawnPoint.z += platformSpawnOffset;
		newObjectSpawnPoint.z += platformSpawnOffset;

		if (GameObject.FindGameObjectWithTag("Player"))
		{
			playerCheck = true;
		}
		else
		{
			playerCheck = false;
		}

		if (playerCheck == true)
		{
			maxPlayerHealth = GameObject.FindGameObjectWithTag ("Player").GetComponent<FirstPersonController> ().maxHealth;
			playerHealth = GameObject.FindGameObjectWithTag ("Player").GetComponent<FirstPersonController> ().playerLife;
		}

		if (playerHealth > maxPlayerHealth)
		{
			playerHealth = maxPlayerHealth;
		}

		EnableFallTrigger ();
		playerCanvas.updateHP(playerHealth, maxPlayerHealth);

		if (GameObject.FindWithTag("AI"))
		{
			bots = GameObject.FindGameObjectsWithTag ("AI");
			botCount = bots.Length;
		}

		if (questionNumber <= (questions.Count - 1))
		{
			test = questions [questionNumber].Split (':');

			//Test type "True or False"
			if (test[1].ToUpper() == "TF")
			{
				//Instantiate Platforms based on test type
				if (firstCreation == true)
				{
					testPlatform = (GameObject)Instantiate (testPrefab_TF, currentObjectSpawnPoint, transform.localRotation, transform);
					testPlatform.gameObject.name = "platform_" + questionNumber;
					firstCreation = false;
					healthSpawned = false;
				}
				else
				{
					if (isCreated == false)
					{
						testPlatform = (GameObject)Instantiate (testPrefab_TF, newObjectSpawnPoint, transform.localRotation, transform);
						testPlatform.gameObject.name = "platform_" + questionNumber;

						notified = false;
						healthSpawned = false;
					}
				}
				isCreated = true;

				currentPlatform = GameObject.Find("platform_" + questionNumber);

				platSpawnT = currentPlatform.transform.Find("Stage Spawn Points").Find("Stage Spawn T");
				platSpawnF = currentPlatform.transform.Find("Stage Spawn Points").Find("Stage Spawn F");

				//Platform Canvas settings
				//Question Canvas
				questionText = currentPlatform.transform.Find("QuestionCanvas").Find("QuestionText").GetComponent<Text>();

				if (questionText.text != test [2])
				{
					questionText.text = test [2];
				}

				//Choices Canvas
				Text choiceTrue = currentPlatform.transform.Find("ChoicesCanvas").Find("Button_True").Find ("Choice_True").GetComponent<Text>();
				Text choiceFalse = currentPlatform.transform.Find("ChoicesCanvas").Find("Button_False").Find ("Choice_False").GetComponent<Text>();

				if (choiceTrue.text != test [3] || choiceFalse.text != test [4])
				{
					choiceTrue.text = test [3];
					choiceFalse.text = test [4];
				}

				if(spawnTrigger.answer != null)
				{
					if (spawnTrigger.answer == test [5])
					{
						//correct answer for TF
						playerScore++;
						time += timeToAdd;
						isCorrect = true;
						botSpawned = false;

						if (bots.Length > 0)
						{
							Destroy (bots [0]);
						}
					}
					else
					{
						if (botCount < maxBots && spawnTrigger.answer != null)
						{
							Transform target = currentPlatform.transform.Find ("PlayerSpawnPoint").GetComponent<Transform> ();
							Vector3 position = target.position + new Vector3 (0, botHeightOffset, 0);

							Instantiate (BotAI, position, player.transform.rotation);
						}

						isCorrect = false;
					}
				}

				GameObject trigT = GameObject.Find("Spawn Trigger T");
				GameObject trigF = GameObject.Find("Spawn Trigger F");

				if (spawnTrigger.answer == "T")
				{
					Debug.Log ("Answered T");
					newObjectSpawnPoint = platSpawnT.transform.position;
					questionNumber++;
					spawnTrigger.answer = null;
					Destroy(trigT);
					Destroy(trigF);
					isCreated = false;
				}
				else if (spawnTrigger.answer == "F")
				{
					Debug.Log ("Answered F");
					newObjectSpawnPoint = platSpawnF.transform.position;
					questionNumber++;
					spawnTrigger.answer = null;
					Destroy(trigT);
					Destroy(trigF);
					isCreated = false;
				}
			}
			else if (test[1].ToUpper() == "MC")
			{
				if (firstCreation == true)
				{
					testPlatform = (GameObject)Instantiate (testPrefab_MC, currentObjectSpawnPoint, transform.localRotation, transform);
					testPlatform.gameObject.name = "platform_" + questionNumber;
					firstCreation = false;
					healthSpawned = false;
				}
				else
				{
					if (isCreated == false)
					{
						testPlatform = (GameObject)Instantiate (testPrefab_MC, newObjectSpawnPoint, transform.localRotation, transform);
						testPlatform.gameObject.name = "platform_" + questionNumber;

						notified = false;
						healthSpawned = false;
					}
				}
				isCreated = true;

				currentPlatform = GameObject.Find("platform_" + questionNumber);

				platSpawnA = currentPlatform.transform.Find("Stage Spawn Points").Find("Stage Spawn A");
				platSpawnB = currentPlatform.transform.Find("Stage Spawn Points").Find("Stage Spawn B");
				platSpawnC = currentPlatform.transform.Find("Stage Spawn Points").Find("Stage Spawn C");

				//Platform Canvas settings
				//Question Canvas
				questionText = currentPlatform.transform.Find("QuestionCanvas").Find("QuestionText").GetComponent<Text>();

				if (questionText.text != test [2])
				{
					questionText.text = test [2];
				}

				//Choices Canvas
				Text choiceA = currentPlatform.transform.Find("ChoicesCanvas").Find("Button_A").Find ("Choice_A").GetComponent<Text>();
				Text choiceB = currentPlatform.transform.Find("ChoicesCanvas").Find("Button_B").Find ("Choice_B").GetComponent<Text>();
				Text ChoiceC = currentPlatform.transform.Find("ChoicesCanvas").Find("Button_C").Find ("Choice_C").GetComponent<Text>();

				if (choiceA.text != test [3] || choiceB.text != test [4] || ChoiceC.text != test [5])
				{
					choiceA.text = test [3];
					choiceB.text = test [4];
					ChoiceC.text = test [5];
				}

				if(spawnTrigger.answer != null){
					if (spawnTrigger.answer == test [6])
					{
						//correct answer for MC
						playerScore++;
						time += timeToAdd;
						isCorrect = true;
						botSpawned = false;

						if (bots.Length > 0)
						{
							Destroy (bots [0]);
						}
					}
					else
					{
						if (botCount < maxBots && spawnTrigger.answer != null)
						{
							Transform target = currentPlatform.transform.Find ("PlayerSpawnPoint").GetComponent<Transform> ();
							Vector3 position = target.position + new Vector3 (0, botHeightOffset, 0);

							Instantiate (BotAI, position, player.transform.rotation);
						}

						isCorrect = false;
					}
				}

				GameObject trigA = GameObject.Find("Spawn Trigger A");
				GameObject trigB = GameObject.Find("Spawn Trigger B");
				GameObject trigC = GameObject.Find("Spawn Trigger C");

				if (spawnTrigger.answer == "A")
				{
					Debug.Log ("Answered A");
					newObjectSpawnPoint = platSpawnA.transform.position;
					questionNumber++;
					spawnTrigger.answer = null;
					Destroy(trigA);
					Destroy(trigB);
					Destroy(trigC);
					isCreated = false;
				}
				else if (spawnTrigger.answer == "B")
				{
					Debug.Log ("Answered B");
					newObjectSpawnPoint = platSpawnB.transform.position;
					questionNumber++;
					spawnTrigger.answer = null;
					Destroy(trigA);
					Destroy(trigB);
					Destroy(trigC);
					isCreated = false;
				}
				else if (spawnTrigger.answer == "C")
				{
					Debug.Log ("Answered C");
					newObjectSpawnPoint = platSpawnC.transform.position;
					questionNumber++;
					spawnTrigger.answer = null;
					Destroy(trigA);
					Destroy(trigB);
					Destroy(trigC);
					isCreated = false;
				}
			}

			//start timer
			if(time > 0 && botCount < maxBots && playerCheck == true)
			{
				time = time-Time.deltaTime;
				isTimeAdded = false;
			}
			else if (time <= 0)
			{
				if (botCount < maxBots && botSpawned == false)
				{
					currentPlatform = GameObject.Find("platform_" + questionNumber);
					Transform target = currentPlatform.transform.Find ("PlayerSpawnPoint").GetComponent<Transform> ();
					Vector3 position = target.position + new Vector3 (0, botHeightOffset, 0);

					Instantiate (BotAI, position, player.transform.rotation);
				}
				botSpawned = true;

				if(isTimeAdded == false){

					// reset time and enable bot spawn again
					time += 60f;
					isTimeAdded = true;
					botSpawned = false;
				}
			}

			//randomly spawn healthbox in each platform
			if (playerHealth < maxPlayerHealth && healthSpawned == false)
			{
				rand = UnityEngine.Random.value;

				Debug.Log ("Random value is: " + rand);

				if (rand > 0.5f && healthSpawned == false)
				{
					healthBox_spawn = currentPlatform.transform.Find ("HealthSpawn");
					Vector3 healthSpawn = healthBox_spawn.transform.position;
					healthSpawn.y += healthBox_yOffset;

					Instantiate (healthBox, healthSpawn, healthBox_spawn.rotation, healthBox_spawn);
				}
			}
			healthSpawned = true;
		}
		else if (questionNumber > (questions.Count - 1))
		{
			platformSpawnOffset = -25;
			newObjectSpawnPoint.z += platformSpawnOffset;
			GameObject endPlatform = GameObject.Find ("Start_End Platform");

			if (GameObject.FindGameObjectWithTag("AI"))
			{
				Destroy (bots [0]);
			}

			if (isCreated == false)
			{
				Debug.Log ("Total Score is: " + playerScore);
				testPlatform = (GameObject)Instantiate (endPlatform, newObjectSpawnPoint, transform.localRotation, transform);
				testPlatform.gameObject.name = "platform_end";

				GameObject finalPlatform = GameObject.Find ("platform_end");
				GameObject.Find ("platform_end").transform.Find ("UI Components").Find ("UICanvas").gameObject.SetActive (true);

				Text Scoring = finalPlatform.transform.Find ("UI Components").Find ("UICanvas").Find("Text").GetComponent<Text> ();

				scoreAverage = (playerScore / totalQuestions) * 100;
				Scoring.text = "Your Total Score is: \n" +  playerScore + " / " + (questions.Count) + "\n Score Percentage: \n" + scoreAverage + "%";

				notified = false;

				//Data to be submitted to the database
				userID = GameController.GetComponent<LoginModule> ().userID;
				username = GameController.GetComponent<LoginModule> ().accountUsername;
				testID = GameController.GetComponent<DBContentProcessor> ().testIndexID;
				rating = scoreAverage.ToString ();
				testMode = GameController.GetComponent<DBContentProcessor> ().testMode;


				StartCoroutine(submitScore (userID, username, testID, rating, testMode));
			}
			isCreated = true;
		}

		if (questionNumber >= 2 && notified == false)
		{
			currentPlatform = GameObject.Find("platform_" + questionNumber);

			//under construction

			Debug.Log ("platform_" + (questionNumber - 2) + "can now be removed.");
		}
		notified = true;
	}

	IEnumerator submitScore(string sf_userID, string sf_username, string sf_testID, string sf_rating, string sf_testMode)
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

	private void EnableFallTrigger()
	{
		if (playerCheck == true)
		{
			//This will find the nearest Fall Trigger
			GameObject[] fallTriggers;
			fallTriggers = GameObject.FindGameObjectsWithTag ("Fall Trigger");
			closest = null;
			float distance = Mathf.Infinity;
			Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().position;
			foreach (GameObject fallTrigger in fallTriggers)
			{
				fallTrigger.GetComponent<spawnTrigger> ().isActive = false;
				Vector3 diff = fallTrigger.transform.position - playerPos;
				float curDistance = diff.sqrMagnitude;
				if (curDistance < distance)
				{
					closest = fallTrigger;
					distance = curDistance;
				}
			}
			closest.GetComponent<spawnTrigger> ().isActive = true;
		}
	}

	public void DialogueMessageControl(string title, string message){
		try{
			DialogPanel.SetActive(true);
		
			DialogTitle.text = title;
			DialogMessage.text = message;
		}catch(MissingReferenceException mre){
		}
	}
}