using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletBehaviour : MonoBehaviour
{
	public float maxFlightTime = 5.0f;

	AudioSource audioSource;

	[SerializeField] AudioClip hitSound;

	void Awake()
	{
		audioSource = gameObject.GetComponent<AudioSource>();
		float projectileStrength = GameObject.FindGameObjectWithTag ("AI").GetComponent<AIBehaviour> ().projectileStrength;

		Rigidbody brb = transform.GetComponent<Rigidbody> ();
		brb.AddRelativeForce (Vector3.forward * projectileStrength, ForceMode.Impulse);
	}

	void Update()
	{
		maxFlightTime -= Time.deltaTime;

		if (maxFlightTime <= 0)
		{
			Destroy (gameObject);
		}
	}

	void OnCollisionEnter(Collision col)
	{
		if (col.transform.tag == "Player")
		{
			
			var playerHealth = GameObject.FindWithTag ("Player").GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ();

			// reduce player hp if more than 0 and return to the nearest spawn point
			if(playerHealth.playerLife > 0){

				var projectile = GameObject.FindGameObjectWithTag("Projectile");

				var playerCanvas_animator = GameObject.Find("BulletHitIndicator").GetComponent<Animator>();
				playerCanvas_animator.SetTrigger("hit");

				playerHealth.playerLife -= 1;
				destroyProjectile();
			}
			
		}

		if (col.transform.tag != "Projectile")
		{
			destroyProjectile();
		}
	}

	void destroyProjectile(){

		audioSource.PlayOneShot(hitSound);
		//gameObject.SetActive(false);

		gameObject.GetComponent<SphereCollider>().enabled = false;

		Destroy(gameObject, hitSound.length);

	}

}
