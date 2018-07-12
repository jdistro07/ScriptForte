using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleController : MonoBehaviour
{
	public float scaleX, scaleY, scaleZ;

	// Use this for initialization
	void Start ()
	{
		scaleX = transform.localScale.x;
		scaleY = transform.localScale.y;
		scaleZ = transform.localScale.z;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (transform.localScale.x > scaleX)
		{
			transform.localScale -= new Vector3(1,0,0);
		}
		else if (transform.localScale.x < scaleX)
		{
			transform.localScale += new Vector3(1,0,0);
		}

		if (transform.localScale.y > scaleY)
		{
			transform.localScale -= new Vector3(0,1,0);
		}
		else if (transform.localScale.y < scaleY)
		{
			transform.localScale += new Vector3(0,1,0);
		}

		if (transform.localScale.z > scaleZ)
		{
			transform.localScale -= new Vector3(0,0,1);
		}
		else if (transform.localScale.z < scaleZ)
		{
			transform.localScale += new Vector3(0,0,1);
		}
	}
}