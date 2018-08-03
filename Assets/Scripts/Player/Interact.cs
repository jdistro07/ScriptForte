using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interact : MonoBehaviour {

    private LayerMask layerMask = ~(1 << 2);
    [SerializeField] private GameObject player;
    [SerializeField] private UnityStandardAssets.Characters.FirstPerson.FirstPersonController playerController;

    [SerializeField] GameObject holdingGameobject;

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
        //scripts
        playerController = new UnityStandardAssets.Characters.FirstPerson.FirstPersonController();

        //animations
        isOpen = canvasObject.GetComponent<Animator>(); //get component to the animator

        //player
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>();
    }

    void Update()
    {
        //raycast
        Debug.DrawRay(this.transform.position, this.transform.forward * RaycastRange, Color.green);

        RaycastHit hit;
        var raycastHit = Physics.Raycast(this.transform.position, this.transform.forward, out hit, RaycastRange, layerMask);

        if(raycastHit){

            //if raycast touches an interactable tagged object
            if (hit.collider.tag == "Interactable")
            {
                Debug.Log("Interactable");
                interactable = true; // interactable object is present!

                if (Input.GetMouseButtonDown(0) && interactable)
                {
                    holdingGameobject = hit.transform.gameObject;

                    //Debug.Log("Mouse 0 pressed");
                    idecanvas.SetActive(true); //open IDE
                    isOpen.SetInteger("isOpen", 1);

                    inputfield.Select();
                    inputfield.ActivateInputField();

                    //disable player controls
                    playerController.walkToggle = false;
                }
            }

            
        }
        if (holdingGameobject)
        { // if holding object is true, update code var from propertiesModifier every frame
            var objModifyer = holdingGameobject.GetComponent<propertiesModifier>();
            objModifyer.code = inputfield.text;
        }

        /*if IDE window is active, disable by pressing esc and enable movements
        *clear gameobject
        */
        if (Input.GetKeyDown(KeyCode.Escape)){
            holdingGameobject = null;
            StartCoroutine(closeIDEAnimation());
        }
    }

    private IEnumerator closeIDEAnimation()
    {
        isOpen.SetInteger("isOpen", 0); // execute animation state and wait to the animation to finish 
        yield return new WaitForSeconds(closeIDE.length);

        // disable canvas and allow player to walk
        idecanvas.SetActive(false);

        //enable walk here
        playerController.walkToggle = true;
    }
}
