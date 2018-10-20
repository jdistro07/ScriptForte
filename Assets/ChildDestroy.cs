using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildDestroy : MonoBehaviour {

	// Use this for initialization
	private void OnDisable()
	{
		foreach (Transform child in transform) {
			GameObject.Destroy(child.gameObject);
		}
	}
}
