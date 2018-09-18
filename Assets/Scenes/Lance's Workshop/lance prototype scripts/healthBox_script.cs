using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class healthBox_script : MonoBehaviour
{

	AudioSource audioSource;
	Animator animator;

	FirstPersonController FPC;

	[SerializeField] AnimationClip animation;
	[SerializeField] AudioClip sfx_healthBox;

	private void OnEnable()
	{

		animator = gameObject.GetComponent<Animator>();
		audioSource = gameObject.GetComponent<AudioSource>();

	}

	void OnTriggerEnter(Collider other)
	{

		GameObject player = GameObject.FindGameObjectWithTag ("Player");
		FPC = player.GetComponent<FirstPersonController> ();


		if (other.transform.tag == "Player")
		{
			if (FPC.playerLife < FPC.maxHealth)
			{
				
				StartCoroutine(animate());

			}
		}
	}

	IEnumerator animate(){

		audioSource.PlayOneShot(sfx_healthBox);

		animator.SetTrigger("PlayerGet");

		this.GetComponent<BoxCollider>().enabled = false;

		FPC.playerLife += 1;

		yield return new WaitForSeconds(animation.length);
		Destroy (gameObject);
		

	}
}