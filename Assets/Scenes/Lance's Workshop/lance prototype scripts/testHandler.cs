using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;

public class testHandler : MonoBehaviour
{
	[Header("Test Questionaire File Path")]
	[SerializeField] private string textPath = "Assets/Scenes/Lance's Workshop/testing.txt";

	[Header("Game Prefabs")]
	[SerializeField] private GameObject player;
	[SerializeField] private GameObject testPrefab_TF;
	[SerializeField] private GameObject testPrefab_MC;

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

	[Header("Scoring and Miscellaneous")]
	[SerializeField] private int questionNumber = 0;
	[SerializeField] private int playerScore = 0;

	[Header("Timer Components")]
	[Range(0,30)]
	public float timeToAdd;

	[Range(0,60)]
	public float time;

	public bool isCorrect;

	private Text questionText;

	private bool firstCreation = true;
	private bool isCreated = false;

	private Vector3 currentObjectSpawnPoint;
	private Vector3 newObjectSpawnPoint;
	[SerializeField] private float platformSpawnOffset = 40;

	[SerializeField] private GameObject testPlatform;

	private void Start()
	{
		if (!File.Exists(textPath))
		{
			File.Create (textPath);
		}

		GameObject playerFind = GameObject.Find ("FPSController");
		Vector3 playerSpawn = GameObject.Find ("PlayerSpawn").GetComponent<Transform> ().position;
		Quaternion playerRotation = GameObject.Find ("PlayerSpawn").GetComponent<Transform> ().rotation;

		if (!playerFind)
		{
			Instantiate (player, playerSpawn, playerRotation);
		}
	}

	private void Awake()
	{
		if (File.Exists(textPath))
		{
			string line;
			StreamReader stream = new StreamReader(textPath);
			while ((line = stream.ReadLine()) != null)
			{
				questions.Add(line);
			}

			stream.Close();
		}
	}

	private void Update()
	{
		currentObjectSpawnPoint = GameObject.Find ("testPlatform_Spawn_Start").GetComponent<Transform> ().transform.position;
		currentObjectSpawnPoint.z += platformSpawnOffset;
		newObjectSpawnPoint.z += platformSpawnOffset;

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
				}
				else
				{
					if (isCreated == false)
					{
						testPlatform = (GameObject)Instantiate (testPrefab_TF, newObjectSpawnPoint, transform.localRotation, transform);
						testPlatform.gameObject.name = "platform_" + questionNumber;
					}
				}
				isCreated = true;

				GameObject currentPlatform = GameObject.Find("platform_" + questionNumber);

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

				if(spawnTrigger.answer != null){
					if (spawnTrigger.answer == test [5])
					{
						//correct answer for TF
						playerScore++;
						time += timeToAdd;
						isCorrect = true;
					}
					else
					{
						isCorrect = false;
					}
				}

				if (spawnTrigger.answer == "T")
				{
					Debug.Log ("Answered T");
					newObjectSpawnPoint = platSpawnT.transform.position;
					questionNumber++;
					spawnTrigger.answer = null;
					GameObject trigT = GameObject.Find("Spawn Trigger T");
					Destroy(trigT);
					isCreated = false;
				}
				else if (spawnTrigger.answer == "F")
				{
					Debug.Log ("Answered F");
					newObjectSpawnPoint = platSpawnF.transform.position;
					questionNumber++;
					spawnTrigger.answer = null;
					GameObject trigF = GameObject.Find("Spawn Trigger F");
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
				}
				else
				{
					if (isCreated == false)
					{
						testPlatform = (GameObject)Instantiate (testPrefab_MC, newObjectSpawnPoint, transform.localRotation, transform);
						testPlatform.gameObject.name = "platform_" + questionNumber;
					}
				}
				isCreated = true;

				GameObject currentPlatform = GameObject.Find("platform_" + questionNumber);

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
					}
					else
					{
						isCorrect = false;
					}
				}

				if (spawnTrigger.answer == "A")
				{
					Debug.Log ("Answered A");
					newObjectSpawnPoint = platSpawnA.transform.position;
					questionNumber++;
					spawnTrigger.answer = null;
					GameObject trigA = GameObject.Find("Spawn Trigger A");
					Destroy(trigA);
					isCreated = false;
				}
				else if (spawnTrigger.answer == "B")
				{
					Debug.Log ("Answered B");
					newObjectSpawnPoint = platSpawnB.transform.position;
					questionNumber++;
					spawnTrigger.answer = null;
					GameObject trigB = GameObject.Find("Spawn Trigger B");
					Destroy(trigB);
					isCreated = false;
				}
				else if (spawnTrigger.answer == "C")
				{
					Debug.Log ("Answered C");
					newObjectSpawnPoint = platSpawnC.transform.position;
					questionNumber++;
					spawnTrigger.answer = null;
					GameObject trigC = GameObject.Find("Spawn Trigger C");
					Destroy(trigC);
					isCreated = false;
				}
			}
		}
		else if (questionNumber > (questions.Count - 1))
		{
			platformSpawnOffset = -25;
			newObjectSpawnPoint.z += platformSpawnOffset;
			GameObject endPlatform = GameObject.Find ("Start_End Platform");

			if (isCreated == false)
			{
				Debug.Log ("Total Score is: " + playerScore);
				testPlatform = (GameObject)Instantiate (endPlatform, newObjectSpawnPoint, transform.localRotation, transform);
				testPlatform.gameObject.name = "platform_end";

				GameObject finalPlatform = GameObject.Find ("platform_end");

				Text Scoring = finalPlatform.transform.Find ("UI Components").Find ("UICanvas").Find("Text").GetComponent<Text> ();
				Scoring.text = "Your Total Score is: \n" +  playerScore + " / " + (questions.Count + 1);
			}
			isCreated = true;
		}

		//start timer
		if(time > 0){
			time = time-Time.deltaTime;
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
