using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour {

	public GameObject prefab;

	void Start()
	{
		Debug.Log(transform.childCount);
	}

	// Update is called once per frame
	void Update () {

		if(transform.childCount == 0){
			var spawnedCube = Instantiate(prefab,transform.position, Quaternion.identity);
			spawnedCube.transform.parent = transform;
		}
	}
}
