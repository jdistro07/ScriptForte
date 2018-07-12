using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseLook : MonoBehaviour
{
	Vector2 mouseLook;
	Vector2 lookSmoothing;
	Vector2 clampInDegrees = new Vector2(360, 180);
	public float sensitivity = 5.0f;
	public float mouseSmoothing = 2.0f;

	public float minX = -90.0f;
	public float maxX = 90.0f;
	public float minY = -60.0f;
	public float maxY = 60.0f;

    public float RaycastRange = 0.0f;

    [Header("GUI")]
    public GameObject idecanvas;
    public InputField inputfield;

	GameObject character;

	// Use this for initialization
	void Start ()
	{
		character = this.transform.parent.gameObject;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Time.timeScale != 0)
		{
			var md = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

			md = Vector2.Scale (md, new Vector2 (sensitivity * mouseSmoothing, sensitivity * mouseSmoothing));
			lookSmoothing.x = Mathf.Lerp (lookSmoothing.x, md.x, 1f / mouseSmoothing);
			lookSmoothing.y = Mathf.Lerp (lookSmoothing.y, md.y, 1f / mouseSmoothing);
			mouseLook += lookSmoothing;

			if (clampInDegrees.x < 360)
			{
				mouseLook.x = Mathf.Clamp (mouseLook.x, minX, minX);
			}

			transform.localRotation = Quaternion.AngleAxis (-mouseLook.y, Vector3.right);
			character.transform.localRotation = Quaternion.AngleAxis (mouseLook.x, character.transform.up);

			if (clampInDegrees.y < 360)
			{
				mouseLook.y = Mathf.Clamp (mouseLook.y, minY, maxY);
			}
		}

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