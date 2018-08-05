using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class door_controller : MonoBehaviour {

    GameObject player;
    public Transform gate;
    public AudioSource sfx;

    void OnTriggerEnter(Collider playerCollision)
    {
        var doorControl = gate.GetComponent<Animator>();

        if(playerCollision.transform.name == "FPSController"){
            doorControl.SetInteger("open", 2);
            sfx.Play();
        }
    }

    void OnTriggerExit(Collider playerCollision2)
    {
        var doorControl = gate.GetComponent<Animator>();
        if (playerCollision2.transform.name == "FPSController")
        {
            doorControl.SetInteger("open", 1);
            sfx.Play();
        }
    }
}
