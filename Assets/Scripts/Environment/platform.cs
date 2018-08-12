using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class platform : MonoBehaviour {
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player"){
            
            other.transform.parent = transform;
            Debug.Log("Player Attached as Child");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        other.transform.parent = null;
        Debug.Log("Player Detached as Child");
    }
}
