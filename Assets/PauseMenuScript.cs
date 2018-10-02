 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class PauseMenuScript : MonoBehaviour
{
	[SerializeField] private Button btnResume;
	[SerializeField] private Button btnSettings;
	[SerializeField] private Button btnLeave;

	UIManager UIManager;
	FirstPersonController fpc;

	void Start()
	{
		UIManager = GameObject.Find ("AIOGameManager").GetComponent<UIManager> ();
		fpc = GameObject.FindGameObjectWithTag ("Player").GetComponent<FirstPersonController> ();

		btnResume.onClick.AddListener (() => {
		
			UIManager.sfxOpen();
			fpc.Resume();
			gameObject.SetActive(false);

		});

		btnSettings.onClick.AddListener (() => {

			UIManager.sfxSpecial();
			//Settings code here...

		});

		btnLeave.onClick.AddListener (() => {

			Time.timeScale = 1;
			Cursor.visible = true;
			fpc.m_MouseLook.lockCursor = false;
			Cursor.lockState = CursorLockMode.None;

			UIManager.sfxClose();
			Initiate.Fade("Main UI", Color.black, 2f);

		});
	}
}
