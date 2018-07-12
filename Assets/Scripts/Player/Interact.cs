using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interact : MonoBehaviour {

    [Header("GUI")]
    public GameObject idecanvas;
    public InputField inputfield;

    public float RaycastRange;

    void Update()
    {
        //raycast
        Debug.DrawRay(this.transform.position, this.transform.forward * RaycastRange, Color.green);

        //references
        var player = GameObject.FindGameObjectWithTag("Player");

        //if raycast touches an interactable tagged object
        if(Physics.Raycast(this.transform.position, this.transform.forward, RaycastRange) && GameObject.FindGameObjectWithTag("Interactable")){
            
            Debug.Log("Interactable object!");

            bool interactable = true;

            if(Input.GetMouseButtonDown(0) && interactable){
                //Debug.Log("Mouse 0 pressed");
                idecanvas.SetActive(true);

                inputfield.Select();
                inputfield.ActivateInputField();

                //disable player controls
                player.GetComponent<CharacterControls>().enabled = false;
            }
        }

        //if IDE window is active, disable by pressing esc and enable movements
        if(Input.GetKeyDown(KeyCode.Escape)){
            idecanvas.SetActive(false);

            //disable player controls
            player.GetComponent<CharacterControls>().enabled = true;
        }
    }

}
