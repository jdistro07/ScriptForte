using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneCamera : MonoBehaviour {

	[SerializeField] GameObject mainCamera;
	Vector3 playerPosition;

	[Header("Values")]
	[SerializeField] Vector3 RotationOffset;
	[SerializeField] Quaternion localRotation;
	[SerializeField] float RotationSpeed;
	
	// Update is called once per frame
	void Update () {
		try{
			Transform player = GameObject.FindWithTag("Player").GetComponent<Transform>();
			int playerHP = GameObject.FindWithTag("Player").GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().playerLife;

			if(player){

				//track position per frame
				playerPosition = player.transform.position;

			}

		}catch{

			//execute game over
			//enable and reposition camera to last player position with developer-defined vector3 values
			if(!mainCamera.activeSelf){

				mainCamera.SetActive(true);

			}

			transform.position = new Vector3(playerPosition.x, playerPosition.y, playerPosition.z);

			//reposition main camera in accordance to the set offset
			mainCamera.transform.localPosition = new Vector3(RotationOffset.x,RotationOffset.y, RotationOffset.z);
			mainCamera.transform.LookAt(playerPosition);
			
			
			//rotate the transform on y axis to complete the game over camera rotation pan effect
			transform.Rotate(0, RotationSpeed * Time.deltaTime, 0);

		}

	}
}
