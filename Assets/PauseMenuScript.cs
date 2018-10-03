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
	[SerializeField] private Transform settingsPanel;

	UIManager UIManager;
	FirstPersonController fpc;

	void Start()
	{
		UIManager = GameObject.Find ("AIOGameManager").GetComponent<UIManager> ();
		settingsPanel = GameObject.Find("PlayerUI_Canvas").transform.Find("InGameSettingsPanel");
		fpc = GameObject.FindGameObjectWithTag ("Player").GetComponent<FirstPersonController> ();

		btnResume.onClick.AddListener (() => {
		
			UIManager.sfxOpen();
			fpc.Resume();
			gameObject.SetActive(false);

		});

		btnSettings.onClick.AddListener (() => {

			UIManager.sfxOpen();
			settingsPanel.gameObject.SetActive(true);

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
