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
			gateAnim.SetInteger("gateOpen", 1);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		gateAnim.SetInteger("gateOpen", 2);
	}
}
