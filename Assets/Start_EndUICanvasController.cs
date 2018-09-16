using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Start_EndUICanvasController : MonoBehaviour {

	DBContentProcessor dbContent_Processor;
	[SerializeField] Button btnFinish;


	// Use this for initialization
	private void OnEnable()
	{
		
		dbContent_Processor = GameObject.Find("AIOGameManager").GetComponent<DBContentProcessor>();

		btnFinish.onClick.AddListener(() => {

			dbContent_Processor.OnClickInGameFinish();

		});


	}
	
}
