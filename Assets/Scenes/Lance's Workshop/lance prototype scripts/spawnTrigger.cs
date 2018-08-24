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
}
