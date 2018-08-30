﻿using System.Collections;
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
	string warning_message = "You have chosen an incorrect Gate. Security protocols are now activating!";
	string warning_title = "Security Warning!";

	[SerializeField] bool isCorrect;

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

		if(!isCorrect){
			dialog.DialogueMessageControl(warning_title,warning_message);
		}
	}
}
