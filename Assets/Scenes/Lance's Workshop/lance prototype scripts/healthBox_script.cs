using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class healthBox_script : MonoBehaviour
{
	void OnTriggerEnter(Collider other)
	{
		GameObject player = GameObject.FindGameObjectWithTag ("Player");
		var FPC = player.GetComponent<FirstPersonController> ();


		if (other.transform.tag == "Player")
		{
			if (FPC.playerLife < FPC.maxHealth)
			{
				FPC.playerLife += 1;
				Destroy (gameObject);
			}
		}
	}
}