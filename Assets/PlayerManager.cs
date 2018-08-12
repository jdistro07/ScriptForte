using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

	[SerializeField] GameObject playerPrefab;
	[SerializeField] GameObject player;

	[Header("Spawn Points in the Area")]
	[SerializeField] GameObject[] spawnPoints;


	private void Start()
	{
		
	}

	void LateUpdate () {
		spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
		player = GameObject.FindGameObjectWithTag("Player");
		if(!player){

		}
	}
}
