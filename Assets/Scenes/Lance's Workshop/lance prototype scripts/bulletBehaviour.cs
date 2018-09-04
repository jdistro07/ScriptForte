using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletBehaviour : MonoBehaviour
{
	public float maxFlightTime = 5.0f;

	void Awake()
	{
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
			Debug.Log ("hit");
		}

		if (col.transform.tag != "Projectile")
		{
			Destroy (gameObject);
		}
	}
}
