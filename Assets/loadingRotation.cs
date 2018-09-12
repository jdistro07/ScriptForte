using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class loadingRotation : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		gameObject.transform.Rotate(new Vector3(0, 0, 10 * Time.deltaTime));
	}
}
