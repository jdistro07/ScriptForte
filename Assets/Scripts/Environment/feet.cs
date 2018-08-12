using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class feet : MonoBehaviour {

	// Update is called once per frame
	void OnCollisionEnter (Collision col) {
		if(col.gameObject.name == "platform"){
            Debug.Log("Stepping on a platform");
        }
	}
}
