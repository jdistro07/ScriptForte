using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class spawnTrigger : MonoBehaviour
{
	AudioSource audioSource;

	[Header("Respawn Settings")]
	[SerializeField] private bool respawnTrigger = false;
	public bool isActive;
	[SerializeField] private Transform spawnPoint;

	[Header("Gate Trigger Settings")]
	[SerializeField] private string answerValue;
	public static string answer;

	[Header("Gameobject")]
	[SerializeField] GameObject testManagerObject;

	Animator crash;
	Animator crashBG;

	testHandler testHandler;
	InGameHelper inGameHelper;

	//variables;
	//wrong answer message
	string warning_message = "You have chosen an incorrect Gate. Security protocols are now activating!";
	string warning_title = "Security Warning!";

	//correct answer message
	[SerializeField] bool isCorrect;
	string correct_message = "You have chosen the correct Gate! Security activation time has been added by ";
	string correct_title = "Correct!";

	//color values
	//YELLOW
	float yr = 255; float yg = 255; float yb = 0; float ya = 255;

	//GREEN
	float gr = 0; float gg = 255; float gb = 0; float ga = 255;

	private void Start()
	{
		
		testManagerObject = GameObject.Find("The Testing Ground");
		testHandler = GameObject.Find("The Testing Ground").GetComponent<testHandler>();
		crash = GameObject.Find("Crash_indicator").GetComponent<Animator>();
		crashBG = GameObject.Find("BulletHitIndicator").GetComponent<Animator>();
		audioSource = gameObject.GetComponent<AudioSource>();
		inGameHelper = GameObject.Find("InGame SubManager").GetComponent<InGameHelper>();

	}

	private void OnTriggerEnter(Collider other)
	{
		spawnPoint = null;
		spawnPoint = testHandler.testPlatform.transform.Find ("PlayerSpawnPoint");

		if (other.transform.tag == "Player")
		{
			if (respawnTrigger == false)
			{
				answer = answerValue;
			}
			else if (respawnTrigger == true && isActive == true)
			{
				Transform player = GameObject.FindWithTag ("Player").GetComponent<Transform> ();
				
				var playerHealth = GameObject.FindWithTag ("Player").GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ();

				// reduce player hp if more than 0 and return to the nearest spawn point
				if(playerHealth.playerLife > 0){

					if(playerHealth.playerLife > 1){

						audioSource.PlayOneShot(inGameHelper.crashSFX);
						crash.SetTrigger("crash");
						crashBG.SetTrigger("hit");
						crashBG.speed = .5f;

					}

					playerHealth.playerLife -= 1;

				}
					
				player.transform.position = spawnPoint.position;
			}
		}
	}

	private void OnDestroy()
	{
		//get value of isCorrect from the testHandler script after a frame
		isCorrect = testManagerObject.GetComponent<testHandler>().isCorrect;
		
		var dialog = testManagerObject.GetComponent<testHandler>();

		try{
			if(!isCorrect){

				dialog.DialogueMessageControl(warning_title,warning_message);
				dialog.DialogTitle.color = new Color(yr, yg, yb, ya);
				
			}else{

				try{
					correct_message = correct_message+" "+testManagerObject.GetComponent<testHandler>().timeToAdd;
				
					dialog.DialogueMessageControl(correct_title,correct_message);
					dialog.DialogTitle.color = new Color(gr, gg, gb, ga);
				}catch(Exception e){
					Debug.Log(e);
				}

			}
		}catch(MissingReferenceException mre){
			Debug.Log("Canvas required do not exist: "+mre);
		}
	}
}
