using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControls : MonoBehaviour
{
	public float speed = 10.0f;
	public float runSpeedMultiplier = 1.50f;
	public float walkSpeedMultiplier = 0.50f;
	public float jumpStrength = 2.0f;

	private Vector3 jump;

	private bool isGrounded;

	Rigidbody rb;

	void Start()
	{
		rb = GetComponent<Rigidbody> ();
		jump = new Vector3 (0.0f, 2.0f, 0.0f);
	}

	void OnCollisionStay()
	{
		isGrounded = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
		float move = (Input.GetAxis ("Vertical") * speed) * Time.deltaTime;
		float strafe = (Input.GetAxis ("Horizontal") * speed) * Time.deltaTime;

		//movement
		if (Input.GetKey (KeyCode.LeftShift))
		{
			transform.Translate (strafe * runSpeedMultiplier, 0, move * runSpeedMultiplier);
		}
		else if (Input.GetKey (KeyCode.LeftAlt))
		{
			transform.Translate (strafe * walkSpeedMultiplier, 0, move * walkSpeedMultiplier);
		}
		else
		{
			transform.Translate (strafe, 0, move);
		}

		if (Input.GetKey (KeyCode.Space) && isGrounded)
		{
			rb.AddForce (jump * jumpStrength, ForceMode.Impulse);
			isGrounded = false;
		}
	}
}