using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

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

    [Header("Audio")]
    AudioSource audioSource_Interact;
    [SerializeField] AudioClip sfx_UIinteractError;

    //Components
    bool interactable;

    //Contributed by Lance
	[SerializeField] private InputField gateInput;
	private bool inputActive = false;

    void Start()
    {
        audioSource_Interact = gameObject.GetComponent<AudioSource>();

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

                //modified testing in standalone is required
                if (Input.GetMouseButtonDown(0) && interactable)
                {
                    try{
                        holdingGameobject = hit.transform.gameObject;

                        //Debug.Log("Mouse 0 pressed");
                        idecanvas.SetActive(true); //open IDE
                        isOpen.SetInteger("isOpen", 1);

                        inputfield.Select();
                        inputfield.ActivateInputField();

                        //disable player controls
                        playerController.walkToggle = false;
                    }catch{

                    }
                    
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

        //Contributed by Lance
		raycastWorldUI();
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

    //Contributed by Lance
    void raycastWorldUI()
    {
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			try{
                PointerEventData pointer = new PointerEventData(EventSystem.current);
                gateInput = GameObject.FindGameObjectWithTag("UIWS Inputfield").GetComponent<InputField>();

                pointer.position = Input.mousePosition;

                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointer, results);

                if (results.Count > 0)
                {
                    gateInput.ActivateInputField();
                    inputActive = true;
                }
            }catch(NullReferenceException nre){
                // play error audio
                audioSource_Interact.PlayOneShot(sfx_UIinteractError);
            }
		}

		if (Input.GetKeyDown(KeyCode.Return))
		{
			gateInput.DeactivateInputField ();
			inputActive = false;
		}

		if (inputActive == true)
		{
			playerController.walkToggle = false;
		}
		else
		{
			playerController.walkToggle = true;
		}
    }
}
