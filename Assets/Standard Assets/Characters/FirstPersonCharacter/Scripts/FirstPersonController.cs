using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using System.IO;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (CharacterController))]
    [RequireComponent(typeof (AudioSource))]
    public class FirstPersonController : MonoBehaviour
    {
        //Modified By Lance
        [SerializeField] private GameObject idecanvas;
        [SerializeField] private InputField gateInput;

		public int maxHealth;
        public int playerLife;
		public Texture2D crosshair;
		private Rect crosshairPos;
		private Transform pauseMenu;

		public bool gamePaused = false;
        public bool walkToggle;
		public MouseLook m_MouseLook;
        [SerializeField] private bool m_IsWalking;
        [SerializeField] private float m_WalkSpeed;
        [SerializeField] private float m_RunSpeed;
        [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
        [SerializeField] private float m_JumpSpeed;
        [SerializeField] private float m_StickToGroundForce;
        [SerializeField] private float m_GravityMultiplier;
        [SerializeField] private bool m_UseFovKick;
        [SerializeField] private FOVKick m_FovKick = new FOVKick();
        [SerializeField] private bool m_UseHeadBob;
        [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
        [SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();
        [SerializeField] private float m_StepInterval;
        [SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
        [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
        [SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.

		[SerializeField] private AudioClip clickSound;

        private Camera m_Camera;
        private bool m_Jump;
        private float m_YRotation;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private Vector3 m_OriginalCameraPosition;
        private float m_StepCycle;
        private float m_NextStep;
        private bool m_Jumping;

        [Header("Audio")]
        private AudioSource m_AudioSource;

        [Header("Customs (Johndel)")]
        bool EnableWalk = true;

        private void Awake()
        {
            try{
                //disable main camera when spawned
                var mainCamera = GameObject.Find("Main Camera");
                if(mainCamera.activeSelf){
                    mainCamera.SetActive(false);
                    Debug.Log("Main Camera disabled!");
                }
            }catch{
                Debug.Log("No Main Camera found!");
            }
            
        }

        // Use this for initialization
        private void Start()
        {
            m_CharacterController = GetComponent<CharacterController>();
            m_Camera = Camera.main;
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_Camera, m_StepInterval);
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle/2f;
            m_Jumping = false;
            m_AudioSource = GetComponent<AudioSource>();
			m_MouseLook.Init(transform , m_Camera.transform);

            walkToggle = true;

            //Modified by Lance
            try{
                gateInput = GameObject.FindGameObjectWithTag("UIWS Inputfield").GetComponent<InputField>();
            }catch(NullReferenceException nre){
                Debug.Log("No inputfield with set tag: UIWS Inputfield");
            }

			pauseMenu = GameObject.Find ("PlayerUI_Canvas").transform.Find ("PauseMenu");
        }


        // Update is called once per frame
        private void Update()
        {
            if(walkToggle){
                m_WalkSpeed = 5f;
                m_JumpSpeed = 10f;
                m_RunSpeed = 10f;
            }else if(!walkToggle){
                m_WalkSpeed = 0f;
                m_JumpSpeed = 0f;
                m_RunSpeed = 0f;
            }

			if (gamePaused == false)
			{
				RotateView();
				// the jump state needs to read here to make sure it is not missed

				//Modified by Lance
				if (!m_Jump)
				{
					m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
				}

				if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
				{
					StartCoroutine(m_JumpBob.DoBobCycle());
					PlayLandingSound();
					m_MoveDir.y = 0f;
					m_Jumping = false;
				}
				if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
				{
					m_MoveDir.y = 0f;
				}

				m_PreviouslyGrounded = m_CharacterController.isGrounded;

				if (Input.GetKeyDown(KeyCode.Mouse0))
				{
					m_AudioSource.PlayOneShot (clickSound);
				}
			}

			if (Input.GetKeyDown(KeyCode.Escape))
			{
				//If game is playing and escape key is pressed
				if (gamePaused == false)
				{
					/*Cursor.visible = true;
					m_MouseLook.lockCursor = true;
					Cursor.lockState = CursorLockMode.None;
					Time.timeScale = 0;
					gamePaused = true;
					Debug.Log ("Game is paused.");*/
					pauseMenu.gameObject.SetActive (true);
					Pause ();
				}
				//If game is paused and escape key is pressed
				else
				{
					/*Cursor.visible = false;
					m_MouseLook.lockCursor = false;
					Cursor.lockState = CursorLockMode.Locked;
					Time.timeScale = 1;
					gamePaused = false;
					Debug.Log ("Game is resumed.");*/
					pauseMenu.gameObject.SetActive (false);
					Resume ();
				}
			}
        }

		public void Pause()
		{
			Cursor.visible = true;
			m_MouseLook.lockCursor = true;
			Cursor.lockState = CursorLockMode.None;
			Time.timeScale = 0;
			gamePaused = true;
			Debug.Log ("Game is paused.");
		}

		public void Resume()
		{
			Cursor.visible = false;
			m_MouseLook.lockCursor = false;
			Cursor.lockState = CursorLockMode.Locked;
			Time.timeScale = 1;
			gamePaused = false;
			Debug.Log ("Game is resumed.");
		}

        private void LateUpdate()
        {
            if(playerLife <= 0){

                //destroy player object
                Destroy(gameObject);

            }
        }


        private void PlayLandingSound()
        {
            m_AudioSource.clip = m_LandSound;
            m_AudioSource.Play();
            m_NextStep = m_StepCycle + .5f;
        }


        private void FixedUpdate()
        {
            float speed;
            GetInput(out speed);
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward*m_Input.y + transform.right*m_Input.x;

            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                               m_CharacterController.height/2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            m_MoveDir.x = desiredMove.x*speed;
            m_MoveDir.z = desiredMove.z*speed;


            if (m_CharacterController.isGrounded)
            {
                m_MoveDir.y = -m_StickToGroundForce;

                if (m_Jump)
                {
                    //Modified By Lance
                    /*if (!idecanvas.activeSelf)
                    {
                        m_MoveDir.y = m_JumpSpeed;
                        PlayJumpSound();
                        m_Jump = false;
                        m_Jumping = true;
                    }*/
					
					m_MoveDir.y = m_JumpSpeed;
					PlayJumpSound();
					m_Jump = false;
					m_Jumping = true;
                }
            }
            else
            {
                m_MoveDir += Physics.gravity*m_GravityMultiplier*Time.fixedDeltaTime;
            }
            m_CollisionFlags = m_CharacterController.Move(m_MoveDir*Time.fixedDeltaTime);

            ProgressStepCycle(speed);
            UpdateCameraPosition(speed);

            m_MouseLook.UpdateCursorLock();
        }
        
        private void PlayJumpSound()
        {
            if(m_JumpSpeed > 0){
                m_AudioSource.clip = m_JumpSound;
                m_AudioSource.Play();
            }
        }


        private void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed*(m_IsWalking ? 1f : m_RunstepLenghten)))*
                             Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep))
            {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }


        private void PlayFootStepAudio()
        {
            if (!m_CharacterController.isGrounded)
            {
                return;
            }
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }


        private void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;
            if (!m_UseHeadBob)
            {
                return;
            }
            if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
            {
                m_Camera.transform.localPosition =
                    m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                      (speed*(m_IsWalking ? 1f : m_RunstepLenghten)));
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
            }
            else
            {
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
            }
            m_Camera.transform.localPosition = newCameraPosition;
        }


        private void GetInput(out float speed)
        {
            // Read input
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");

            bool waswalking = m_IsWalking;

#if !MOBILE_INPUT
            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running
            m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
#endif
            // set the desired speed to be walking or running
            speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }

            // handle speed change to give an fov kick
            // only if the player is going to a run, is running and the fovkick is to be used
            if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
            {
                StopAllCoroutines();
                StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
            }
        }


        private void RotateView()
        {
            m_MouseLook.LookRotation (transform, m_Camera.transform);
        }


        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(m_CharacterController.velocity*0.1f, hit.point, ForceMode.Impulse);
        }

		private void OnGUI()
		{
			if (gamePaused == false)
			{
				float xMin = (Screen.width / 2) - (crosshair.width / 2);
				float yMin = (Screen.height / 2) - (crosshair.height / 2);
				GUI.DrawTexture (new Rect (xMin, yMin, crosshair.width, crosshair.height), crosshair);
			}
		}
    }
}
