using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingSceneController : MonoBehaviour {

	
	[Header("UI Controls"),SerializeField] Button start;
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

			uiManager.sfxSpecial();
			dbProcessor.StartGame();

		});

	}

	

}
