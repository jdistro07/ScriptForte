using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Start_EndUICanvasController : MonoBehaviour {

	[SerializeField] DBContentProcessor dbContent_Processor;
	[SerializeField] UIManager uiManager;

	[SerializeField] Button btnFinish;


	// Use this for initialization
	private void Awake()
	{
		
		dbContent_Processor = GameObject.Find("AIOGameManager").GetComponent<DBContentProcessor>();
		uiManager = GameObject.Find("AIOGameManager").GetComponent<UIManager>();

		btnFinish.onClick.AddListener(() => {

			Debug.Log("Leaving world");

			uiManager.sfxSpecial();
			dbContent_Processor.OnClickInGameFinish();
			Debug.Log("Returning to main menu");

		});


	}
	
}
