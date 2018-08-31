using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnTrigger : MonoBehaviour
{
	[Header("Respawn Settings")]
	[SerializeField] private bool respawnTrigger = false;
	[SerializeField] private Transform spawnPoint;

	[Header("Gate Trigger Settings")]
	[SerializeField] private string answerValue;
	public static string answer;

	[Header("Gameobject")]
	[SerializeField] GameObject testManagerObject;

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
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.tag == "Player")
		{
			//normal gate trigger
			answer = answerValue;
			//close gate

			//player fall respawn trigger
			if (respawnTrigger == true)
			{
				Transform player = GameObject.FindWithTag ("Player").GetComponent<Transform> ();
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

				Debug.Log("Incorrect answer.");
			}else{
				dialog.DialogueMessageControl(correct_title,correct_message);
				dialog.DialogTitle.color = new Color(gr, gg, gb, ga);

				Debug.Log("Correct answer");
			}
		}catch(MissingReferenceException mre){
			Debug.Log("Canvas required do not exist: "+mre);
		}
	}
}
