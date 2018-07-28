using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interact : MonoBehaviour {

    private LayerMask layerMask = ~(1 << 2);

    [Header("GUI")]
    public GameObject idecanvas;
    public InputField inputfield;

    public float RaycastRange;

    [Header("Animations")]
    private Animator isOpen;
    public AnimationClip closeIDE;
    public Canvas canvasObject;

    //Components
    bool interactable;

    void Start()
    {
        isOpen = canvasObject.GetComponent<Animator>(); //get component to the animator
    }

    void Update()
    {
        //raycast
        Debug.DrawRay(this.transform.position, this.transform.forward * RaycastRange, Color.green);

        //references
        var player = GameObject.FindGameObjectWithTag("Player");
        RaycastHit hit;
        var raycastHit = Physics.Raycast(this.transform.position, this.transform.forward, out hit, RaycastRange, layerMask);

        if(raycastHit){

            //if raycast touches an interactable tagged object
            if (hit.collider.tag == "Interactable")
            {
                Debug.Log("Interactable");
                interactable = true; // interactable objeect is present!

                if (Input.GetMouseButtonDown(0) && interactable)
                {
                    //Debug.Log("Mouse 0 pressed");
                    idecanvas.SetActive(true); //open IDE
                    isOpen.SetInteger("isOpen", 1);

                    inputfield.Select();
                    inputfield.ActivateInputField();

                    //disable player controls
                    player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;
                }
            }
        }

        //if IDE window is active, disable by pressing esc and enable movements
        if(Input.GetKeyDown(KeyCode.Escape)){
            StartCoroutine(closeIDEAnimation());
        }
    }

    private IEnumerator closeIDEAnimation()
    {
        isOpen.SetInteger("isOpen", 0); // execute animation state and wait to the animation to finish 
        yield return new WaitForSeconds(closeIDE.length);

        // disable canvas and allow player to walk
        var player = GameObject.FindGameObjectWithTag("Player");
        idecanvas.SetActive(false);
        player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = true;
    }

}
