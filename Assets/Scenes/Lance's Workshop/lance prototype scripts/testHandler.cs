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
	[SerializeField] private bool randomizeQuestions;

	[SerializeField] private string testType;
	[SerializeField, Range(0, 100)] private float fetchLimit;

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
	[SerializeField] private List<String> randomizer;
	[SerializeField] private string[] test;

	[Header("Answer Key")]
	public Transform ansKeySpawn;
	public GameObject answerKey;
	public float ansKeyOffset;
	//Miscellaneous
	[SerializeField] private GameObject ansKey;
	[SerializeField] private GameObject currentAns;
	private Transform ansKeyQuestion;
	private Transform ansKeyPlayerAns;
	private Transform ansKeyCorrectAns;
	private Vector3 currentAnsKeySpawnPoint;
	private Vector3 newAnsKeySpawnPoint;

	RectTransform contentRectTransform;

	[Header("Scoring")]
	[SerializeField] private int questionNumber = 0;
	public double totalQuestions;
	public double playerScore = 0;
	public double scoreAverage;
	public int correctCount;
	public int mistakeCount;

	[Header("AI")]
	[SerializeField] private GameObject[] bots;
	[SerializeField] private float botHeightOffset = 10;
	[SerializeField] private int maxBots = 4;
	private int botCount;
	private bool botsEnabled;

	[Header("Timer Components")]
	[Range(0,30)]
	public float timeToAdd;
	bool isTimeAdded;
	private bool timerStarted = false;

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
	public GameObject testPlatform;
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
		Vector3 playerSpawn = GameObject.Find ("PlayerSpawnPoint").GetComponent<Transform> ().position;
		Quaternion playerRotation = GameObject.Find ("PlayerSpawnPoint").GetComponent<Transform> ().rotation;

		GameObject.Find ("Start_End Platform").transform.Find ("UI Components").Find ("UICanvas").gameObject.SetActive(false);
		GameObject.Find ("Start_End Platform").transform.Find ("sceneTrigger").gameObject.SetActive (false);

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
		contentRectTransform = GameObject.Find ("PlayerUI_Canvas").transform.Find ("LoadingCanvas").Find ("Scroll View").Find ("Viewport").Find ("Content").GetComponent<RectTransform> ();

		test = questionsFetch.Split (new String[] {"~"}, StringSplitOptions.RemoveEmptyEntries);

		/*
		 * Determine whether the test type is "pre-test" or "post-test" and if the randomizer is set or not.
		 * Randomizer Logic:
		 * 1. Fetch and add each questions as items in a list in "List<String> randomizer".
		 * 		- This will list all questions in order in the randomizer list.
		 * 2. Randomly pick questions from the randomizer list to the "List<String> questions" as a final question sequence.
		 * 		- The questions list is the actual questions that is shown in-game.
		 * 3. Remove the items from the randomizer list that was added to the questions list.
		 * 		- This is to prevent the questions from repeating.
		*/
		if (testType == "PRE")
		{
			if (randomizeQuestions == false)
			{
				for (int x = 0; x < Mathf.Round(test.Length * (fetchLimit / 100)) ; x++)
				{
					questions.Add (test [x]);
				}
			}
			else if (randomizeQuestions == true)
			{
				for (int x = 0; x < Mathf.Round(test.Length * (fetchLimit / 100)) ; x++)
				{
					randomizer.Add (test [x]);
				}

				if (randomizeQuestions == true && randomizer.Count > 0)
				{
					while (randomizer.Count > 0)
					{
						int val = UnityEngine.Random.Range (0, (randomizer.Count - 1));
						questions.Add (randomizer [val]);
						randomizer.RemoveAt (val);
					}
				}
			}

			botsEnabled = false;
		}
		else if (testType == "POST")
		{
			if (randomizeQuestions == false)
			{
				for (int x = 0; x < test.Length; x++)
				{
					questions.Add (test [x]);
				}
			}
			else if (randomizeQuestions == true)
			{
				for (int x = 0; x < test.Length; x++)
				{
					randomizer.Add (test [x]);
				}

				if (randomizeQuestions == true && randomizer.Count > 0)
				{
					while (randomizer.Count > 0)
					{
						int val = UnityEngine.Random.Range (0, (randomizer.Count - 1));
						questions.Add (randomizer [val]);
						randomizer.RemoveAt (val);
					}
				}
			}

			botsEnabled = true;
		}

		Debug.Log ("total number of questions added: " + questions.Count);
		totalQuestions = questions.Count;
	}

	private void Update()
	{
		//Find where the first platform will spawn.
		currentObjectSpawnPoint = GameObject.Find ("testPlatform_Spawn_Start").GetComponent<Transform> ().transform.position;

		//Offsets determines how far the platform will spawn from the spawn point.
		currentObjectSpawnPoint.z += platformSpawnOffset;
		newObjectSpawnPoint.z += platformSpawnOffset;

		currentAnsKeySpawnPoint = ansKeySpawn.transform.position;

		if (isCreated == true)
		{
			newAnsKeySpawnPoint = currentAns.transform.position;
			newAnsKeySpawnPoint.y -= ansKeyOffset;
		}

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

		/*
		 * This is the actual logic of the test handler.
		 * If there is a question fetched, execute code.
		*/
		if (questionNumber <= (questions.Count - 1))
		{
			//Splits the single string data that was fetched into an array of questions.
			test = questions [questionNumber].Split (':');

			//Test type "True or False"
			if (test[1].ToUpper() == "TF")
			{
				//Instantiate Platforms based on test type
				if (firstCreation == true)
				{
					testPlatform = (GameObject)Instantiate (testPrefab_TF, currentObjectSpawnPoint, transform.localRotation, transform);
					testPlatform.gameObject.name = "platform_" + questionNumber;

					ansKey = (GameObject)Instantiate (answerKey, currentAnsKeySpawnPoint, ansKeySpawn.localRotation, ansKeySpawn.parent);
					ansKey.gameObject.name = "answer_" + questionNumber;

					firstCreation = false;
					healthSpawned = false;
				}
				else
				{
					if (isCreated == false)
					{
						testPlatform = (GameObject)Instantiate (testPrefab_TF, newObjectSpawnPoint, transform.localRotation, transform);
						testPlatform.gameObject.name = "platform_" + questionNumber;

						ansKey = (GameObject)Instantiate (answerKey, newAnsKeySpawnPoint, ansKeySpawn.localRotation, ansKeySpawn.parent);
						ansKey.gameObject.name = "answer_" + questionNumber;

						/*float ansKeyRectHeight = ansKey.GetComponent<RectTransform> ().rect.height;
						float rectHeight = contentRectTransform.rect.height;
						contentRectTransform.sizeDelta = new Vector2 (contentRectTransform.rect.width, rectHeight += ansKeyRectHeight);*/

						notified = false;
						healthSpawned = false;
					}
				}
				isCreated = true;

				currentPlatform = GameObject.Find("platform_" + questionNumber);
				currentAns = GameObject.Find ("answer_" + questionNumber);

				platSpawnT = currentPlatform.transform.Find("Stage Spawn Points").Find("Stage Spawn T");
				platSpawnF = currentPlatform.transform.Find("Stage Spawn Points").Find("Stage Spawn F");

				//Platform Canvas settings
				//Question Canvas
				questionText = currentPlatform.transform.Find("QuestionCanvas").Find("QuestionText").GetComponent<Text>();
				ansKeyQuestion = currentAns.transform.Find ("Questions").Find ("txtQuestions");
				ansKeyPlayerAns = currentAns.transform.Find ("playerAnswer").Find ("txtPlayerAnswer");
				ansKeyCorrectAns = currentAns.transform.Find ("correctAnswer").Find ("txtCorrectAnswer");

				if (questionText.text != test [2])
				{
					questionText.text = test [2];
				}

				Text quesAnstxt = ansKeyQuestion.GetComponent<Text> ();
				Text corAnstxt = ansKeyCorrectAns.GetComponent<Text> ();

				if (quesAnstxt.text != test [2] || corAnstxt.text != test [5])
				{
					quesAnstxt.text = (questionNumber + 1) + ". " + test [2];

					if (test [5] == "T")
					{
						corAnstxt.text = test [3];
					}
					else if (test [5] == "F")
					{
						corAnstxt.text = test [4];
					}

					corAnstxt.color = new Color (0, 255, 0);
				}

				//Choices Canvas
				Text choiceTrue = currentPlatform.transform.Find("ChoicesCanvas").Find("Button_True").Find ("Choice_True").GetComponent<Text>();
				Text choiceFalse = currentPlatform.transform.Find("ChoicesCanvas").Find("Button_False").Find ("Choice_False").GetComponent<Text>();

				//Displays the choices in the choices panel on top of the gates.
				if (choiceTrue.text != test [3] || choiceFalse.text != test [4])
				{
					choiceTrue.text = test [3];
					choiceFalse.text = test [4];
				}

				/*
				 * If the answer is correct:
				 * - Add score
				 * - Add more time to the timer
				 * - Destroy 1 bot that is chasing the player
				 * 
				 * If the answer is wrong:
				 * - Spawn 1 AI to chase down the player
				*/
				if(spawnTrigger.answer != null)
				{
					Text plyrAnstxt = ansKeyPlayerAns.GetComponent<Text> ();

					if (spawnTrigger.answer == test [5])
					{
						//correct answer for TF
						playerScore++;
						correctCount++;

						if (botsEnabled == true)
						{
							time += timeToAdd;
						}

						isCorrect = true;
						botSpawned = false;

						if (bots.Length > 0)
						{
							Destroy (bots [0]);
						}

						quesAnstxt.color = new Color (0, 255, 0);
						plyrAnstxt.color = new Color (0, 255, 0);
					}
					else
					{
						if (botCount < maxBots && spawnTrigger.answer != null && botsEnabled == true)
						{
							Transform target = currentPlatform.transform.Find ("PlayerSpawnPoint").GetComponent<Transform> ();
							Vector3 position = target.position + new Vector3 (0, botHeightOffset, 0);

							Instantiate (BotAI, position, player.transform.rotation);
						}

						playerScore -= 1;
						isCorrect = false;

						quesAnstxt.color = new Color (255, 0, 0);
						plyrAnstxt.color = new Color (255, 0, 0);
					}

					if (timerStarted == false && botsEnabled == true)
					{
						timerStarted = true;
					}
				}

				GameObject trigT = GameObject.Find("Spawn Trigger T");
				GameObject trigF = GameObject.Find("Spawn Trigger F");

				//If the player enters the spawnTrigger this will remove all the triggers in the current platform to prevent triggering over again.
				if (spawnTrigger.answer == "T")
				{
					Text plyrAnstxt = ansKeyPlayerAns.GetComponent<Text> ();
					newObjectSpawnPoint = platSpawnT.transform.position;
					questionNumber++;
					spawnTrigger.answer = null;
					Destroy(trigT);
					Destroy(trigF);
					plyrAnstxt.text = test [3];
					isCreated = false;
				}
				else if (spawnTrigger.answer == "F")
				{
					Text plyrAnstxt = ansKeyPlayerAns.GetComponent<Text> ();
					newObjectSpawnPoint = platSpawnF.transform.position;
					questionNumber++;
					spawnTrigger.answer = null;
					Destroy(trigT);
					Destroy(trigF);
					plyrAnstxt.text = test [4];
					isCreated = false;
				}
			}
			else if (test[1].ToUpper() == "MC")
			{
				if (firstCreation == true)
				{
					testPlatform = (GameObject)Instantiate (testPrefab_MC, currentObjectSpawnPoint, transform.localRotation, transform);
					testPlatform.gameObject.name = "platform_" + questionNumber;

					ansKey = (GameObject)Instantiate (answerKey, currentAnsKeySpawnPoint, ansKeySpawn.localRotation, ansKeySpawn.parent);
					ansKey.gameObject.name = "answer_" + questionNumber;

					firstCreation = false;
					healthSpawned = false;
				}
				else
				{
					if (isCreated == false)
					{
						testPlatform = (GameObject)Instantiate (testPrefab_MC, newObjectSpawnPoint, transform.localRotation, transform);
						testPlatform.gameObject.name = "platform_" + questionNumber;

						ansKey = (GameObject)Instantiate (answerKey, newAnsKeySpawnPoint, ansKeySpawn.localRotation, ansKeySpawn.parent);
						ansKey.gameObject.name = "answer_" + questionNumber;

						/*float ansKeyRectHeight = ansKey.GetComponent<RectTransform> ().rect.height;
						float rectHeight = contentRectTransform.rect.height;
						contentRectTransform.sizeDelta = new Vector2 (contentRectTransform.rect.width, rectHeight += ansKeyRectHeight);*/

						notified = false;
						healthSpawned = false;
					}
				}
				isCreated = true;

				currentPlatform = GameObject.Find("platform_" + questionNumber);
				currentAns = GameObject.Find ("answer_" + questionNumber);

				platSpawnA = currentPlatform.transform.Find("Stage Spawn Points").Find("Stage Spawn A");
				platSpawnB = currentPlatform.transform.Find("Stage Spawn Points").Find("Stage Spawn B");
				platSpawnC = currentPlatform.transform.Find("Stage Spawn Points").Find("Stage Spawn C");

				//Platform Canvas settings
				//Question Canvas
				questionText = currentPlatform.transform.Find("QuestionCanvas").Find("QuestionText").GetComponent<Text>();
				ansKeyQuestion = currentAns.transform.Find ("Questions").Find ("txtQuestions");
				ansKeyPlayerAns = currentAns.transform.Find ("playerAnswer").Find ("txtPlayerAnswer");
				ansKeyCorrectAns = currentAns.transform.Find ("correctAnswer").Find ("txtCorrectAnswer");

				if (questionText.text != test [2])
				{
					questionText.text = test [2];
				}

				Text quesAnstxt = ansKeyQuestion.GetComponent<Text> ();
				Text corAnstxt = ansKeyCorrectAns.GetComponent<Text> ();

				if (quesAnstxt.text != test [2] || corAnstxt.text != test [6])
				{
					quesAnstxt.text = (questionNumber + 1) + ". " + test [2];

					if (test [6] == "A")
					{
						corAnstxt.text = test [3];
					}
					else if (test [6] == "B")
					{
						corAnstxt.text = test [4];
					}
					else if (test [6] == "C")
					{
						corAnstxt.text = test [5];
					}

					corAnstxt.color = new Color (0, 225, 0);
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

				if(spawnTrigger.answer != null)
				{
					Text plyrAnstxt = ansKeyPlayerAns.GetComponent<Text> ();

					if (spawnTrigger.answer == test [6])
					{
						//correct answer for MC
						playerScore++;
						correctCount++;

						if (botsEnabled == true)
						{
							time += timeToAdd;
						}

						isCorrect = true;
						botSpawned = false;

						if (bots.Length > 0)
						{
							Destroy (bots [0]);
						}

						quesAnstxt.color = new Color (0, 225, 0);
						plyrAnstxt.color = new Color (0, 225, 0);
					}
					else
					{
						if (botCount < maxBots && spawnTrigger.answer != null && botsEnabled == true)
						{
							Transform target = currentPlatform.transform.Find ("PlayerSpawnPoint").GetComponent<Transform> ();
							Vector3 position = target.position + new Vector3 (0, botHeightOffset, 0);

							Instantiate (BotAI, position, player.transform.rotation);
						}

						playerScore -= 0.5;
						isCorrect = false;

						quesAnstxt.color = new Color (255, 0, 0);
						plyrAnstxt.color = new Color (255, 0, 0);
					}

					if (timerStarted == false && botsEnabled == true)
					{
						timerStarted = true;
					}
				}

				GameObject trigA = GameObject.Find("Spawn Trigger A");
				GameObject trigB = GameObject.Find("Spawn Trigger B");
				GameObject trigC = GameObject.Find("Spawn Trigger C");

				if (spawnTrigger.answer == "A")
				{
					Text plyrAnstxt = ansKeyPlayerAns.GetComponent<Text> ();
					newObjectSpawnPoint = platSpawnA.transform.position;
					questionNumber++;
					spawnTrigger.answer = null;
					Destroy(trigA);
					Destroy(trigB);
					Destroy(trigC);
					plyrAnstxt.text = test [3];
					isCreated = false;
				}
				else if (spawnTrigger.answer == "B")
				{
					Text plyrAnstxt = ansKeyPlayerAns.GetComponent<Text> ();
					newObjectSpawnPoint = platSpawnB.transform.position;
					questionNumber++;
					spawnTrigger.answer = null;
					Destroy(trigA);
					Destroy(trigB);
					Destroy(trigC);
					plyrAnstxt.text = test [4];
					isCreated = false;
				}
				else if (spawnTrigger.answer == "C")
				{
					Text plyrAnstxt = ansKeyPlayerAns.GetComponent<Text> ();
					newObjectSpawnPoint = platSpawnC.transform.position;
					questionNumber++;
					spawnTrigger.answer = null;
					Destroy(trigA);
					Destroy(trigB);
					Destroy(trigC);
					plyrAnstxt.text = test [5];
					isCreated = false;
				}
			}

			//start timer
			if (botsEnabled == true)
			{
				if (timerStarted == true)
				{
					if(time > 0 && botCount < maxBots && playerCheck == true)
					{
						time = time-Time.deltaTime;
						isTimeAdded = false;
					}
					else if (time <= 0)
					{
						//If timer runs out and the maximum number of bots is not reached, spawn 1 AI.
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
		//If all questions are answered
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
				GameObject.Find ("platform_end").transform.Find ("sceneTrigger").gameObject.SetActive (true);

				Text Scoring = finalPlatform.transform.Find ("UI Components").Find ("UICanvas").Find("Text").GetComponent<Text> ();

				scoreAverage = (playerScore / totalQuestions) * 100;
				Scoring.text = "Your Total Score is: \n" +  playerScore + " / " + (questions.Count) + "\n Score Percentage: \n" + scoreAverage.ToString("F2") + "%";

				notified = false;
			}
			isCreated = true;
		}

		if (playerScore < 0)
		{
			playerScore = 0;
		}

		if (questionNumber >= 2 && notified == false)
		{
			currentPlatform = GameObject.Find("platform_" + questionNumber);

			//under construction

			Debug.Log ("platform_" + (questionNumber - 2) + "can now be removed.");
		}
		notified = true;
	}

	//This will look for the fall trigger closest to the player and activates it. This will prevent the player from falling completely off the play field.
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
		}catch{
		}
	}
}
