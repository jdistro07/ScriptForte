using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBehaviour : MonoBehaviour
{
	public GameObject target;
	public float moveSpeed;
	public float followOffset;

	private Vector3 targetPos;
	[SerializeField] private int botCount;

	void Update()
	{
		//AI Count
		GameObject[] bots = GameObject.FindGameObjectsWithTag ("AI");
		botCount = bots.Length;

		//Player Targeting
		target = GameObject.FindWithTag ("Player");
		targetPos = new Vector3 (target.transform.position.x, transform.position.y, target.transform.position.z);
		transform.position = Vector3.MoveTowards (transform.position, targetPos, moveSpeed * Time.deltaTime);
	}
}
