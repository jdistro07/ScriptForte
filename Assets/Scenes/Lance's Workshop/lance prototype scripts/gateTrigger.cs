using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gateTrigger : MonoBehaviour
{
	[SerializeField] private Animator gateAnim;
	[SerializeField] private GameObject platformSpawn;

	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.tag == "Player")
		{
			gateAnim.enabled = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		//close gate
	}
}
