using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingSceneController : MonoBehaviour {

	
	[Header("UI Controls"),SerializeField] Button start;
	[SerializeField] Text txtStart;
	Animator LoadingCanvasAnimator;

	private void Awake()
	{
		LoadingCanvasAnimator = gameObject.GetComponent<Animator>();
	}

	// Use this for initialization
	void Start () {
		
		DBContentProcessor dbProcessor = GameObject.Find("AIOGameManager").GetComponent<DBContentProcessor>();
		UIManager uiManager= GameObject.Find("AIOGameManager").GetComponent<UIManager>();

		start.onClick.AddListener(() => {

			LoadingCanvasAnimator.SetTrigger("fadeOut");

			start.interactable = false;
			txtStart.text = "STARTING SIMULATION...";

			uiManager.sfxSpecial();
			dbProcessor.StartGame();

		});

	}

	

}
