using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Start_EndUICanvasController : MonoBehaviour {

	[SerializeField]DBContentProcessor dbContent_Processor;
	[SerializeField]UIManager uiManager;

	[SerializeField] Button btnFinish;


	// Use this for initialization
	private void Start()
	{
		
		dbContent_Processor = GameObject.Find("AIOGameManager").GetComponent<DBContentProcessor>();
		uiManager = GameObject.Find("AIOGameManager").GetComponent<UIManager>();

		btnFinish.onClick.AddListener(() => {

			uiManager.sfxSpecial();
			dbContent_Processor.OnClickInGameFinish();
			Debug.Log("Returning to main menu");

		});


	}
	
}
