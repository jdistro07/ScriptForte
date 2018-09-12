using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnTest : MonoBehaviour {

	public GameObject spawnables;

	// Use this for initialization
	void Start () {
		Instantiate(spawnables,transform.position,transform.rotation);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
